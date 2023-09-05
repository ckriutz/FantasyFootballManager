using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using System.Text.Json;

namespace FantasyFootballManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FantasyPlayerController : ControllerBase
{
    private readonly ILogger<FantasyPlayerController> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly SearchCommands _ft;

    public FantasyPlayerController(ILogger<FantasyPlayerController> logger, IConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        _ft = _connectionMultiplexer.GetDatabase().FT();
    }

    [HttpGet("{id}")]
    public FantasyPlayer GetPlayer(string id)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;
        return player;
    }

    [HttpGet("search/{query}")]
    public IEnumerable<FantasyPlayer> SearchPlayers(string query)
    {
        List<FantasyPlayer> players = new List<FantasyPlayer>();
        string queryString = $"@{query}";

        foreach (var doc in _ft.Search("idxPlayers", new Query(queryString).Limit(0, 1700)).ToJson())
        {
            FantasyPlayer p = JsonSerializer.Deserialize<FantasyPlayer>(doc)!;
            players.Add(p);
        }
        _logger.LogInformation($"Found {players.Count} players");

        return players;
    }

    [HttpGet]
    public IEnumerable<FantasyPlayer> GetPlayers()
    {
        List<FantasyPlayer> players = new List<FantasyPlayer>();

        foreach (var doc in _ft.Search("idxPlayers", new Query($"*").Limit(0,1700)).ToJson())
        {
            FantasyPlayer p = JsonSerializer.Deserialize<FantasyPlayer>(doc)!;
            players.Add(p);
        }
        _logger.LogInformation($"Found {players.Count} players");

        return players;
    }

    [HttpGet("myplayers")]
    public IEnumerable<FantasyPlayer> GetMyPlayers()
    {
        List<FantasyPlayer> players = new List<FantasyPlayer>();

        foreach (var doc in _ft.Search("idxPlayers", new Query($"*").Limit(0,1700)).ToJson())
        {
            FantasyPlayer p = JsonSerializer.Deserialize<FantasyPlayer>(doc)!;
            if(p.IsOnMyTeam)
            {
                players.Add(p);
            }
        }
        _logger.LogInformation($"Found {players.Count} players on my team.");

        return players;
    }

    [HttpPost("assign/{id}")]
    public IActionResult AssignPlayer(string id)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        _logger.LogInformation($"Assigning player {player.FullName} to someone elses team.");

        player.IsOnMyTeam = false;
        player.IsTaken = true;

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
            return Ok();
        else
            return BadRequest();
    }

    [HttpPost("claim/{id}")]
    public IActionResult ClaimPlayer(string id)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        _logger.LogInformation($"Claiming player {player.FullName} for my team.");

        player.IsOnMyTeam = true;
        player.IsTaken = false;

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
            return Ok();
        else
            return BadRequest();
    }

    [HttpPost("release/{id}")]
    public IActionResult ReleasePlayer(string id)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        _logger.LogInformation($"Releasing player {player.FullName}");

        player.IsOnMyTeam = false;
        player.IsTaken = false;

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
            return Ok();
        else
            return BadRequest();
    }

    [HttpPost("thumbsup/{id}")]
    public IActionResult ThumbsUpPlayer(string id)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        if(player.IsThumbsUp)
        {
            player.IsThumbsUp = false;
            _logger.LogInformation($"Setting {player.FullName} to nutural.");
        }
        else
        {
            player.IsThumbsUp = true;
            _logger.LogInformation($"Liking player {player.FullName}");
        }

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
            return Ok();
        else
            return BadRequest();
    }

    [HttpPost("thumbsdown/{id}")]
    public IActionResult ThumbsDownPlayer(string id)
    {
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        if(player.IsThumbsDown)
            player.IsThumbsDown = false;
        else
            player.IsThumbsDown = true;

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
            return Ok();
        else
            return BadRequest();
    }

    public SearchCommands GetPlayerIndex()
    {
        _logger.LogInformation("Building Player Index");
        IDatabase db = _connectionMultiplexer.GetDatabase();
        SearchCommands ft = db.FT();

        try {ft.DropIndex("idxPlayers");} catch {};
        ft.Create("idxPlayers", new FTCreateParams().On(IndexDataType.JSON).Prefix("player:"), new Schema()
            .AddNumericField(new FieldName("$.id", "id"))
            .AddTextField(new FieldName("$.SleeperId", "SleeperId"))
            .AddTextField(new FieldName("$.FullName", "FullName"))
            .AddTextField(new FieldName("$.Position", "Position")));

        return ft;
    }
}