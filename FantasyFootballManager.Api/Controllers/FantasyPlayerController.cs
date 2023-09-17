using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using System.Text.Json;
using System.Diagnostics;

namespace FantasyFootballManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FantasyPlayerController : ControllerBase
{
    private readonly ILogger<FantasyPlayerController> _logger;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly SearchCommands _ft;
    private readonly ActivitySource _activitySource;

    public FantasyPlayerController(ILogger<FantasyPlayerController> logger, IConnectionMultiplexer connectionMultiplexer, ActivitySource activitySource)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        _ft = _connectionMultiplexer.GetDatabase().FT();
        _activitySource = activitySource;
    }

    [HttpGet("{id}")]
    public FantasyPlayer GetPlayer(string id)
    {
        using var activity = _activitySource.StartActivity("Get Fantasy Player");
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;
        if(player != null)
        {
            activity?.SetTag("player_found", true);
            activity?.SetTag("player", player.FullName);
            activity?.SetTag("playerId", player.SleeperId);
            return player;
        }
        else
        {
            activity?.SetTag("player_found", false);
            return new FantasyPlayer();
        }
        
    }

    [HttpGet("search/{query}")]
    public IEnumerable<FantasyPlayer> SearchPlayers(string query)
    {
        using var activity = _activitySource.StartActivity("Search Players");
        List<FantasyPlayer> players = new List<FantasyPlayer>();
        string queryString = $"@{query}";

        foreach (var doc in _ft.Search("idxPlayers", new Query(queryString).Limit(0, 1700)).ToJson())
        {
            FantasyPlayer p = JsonSerializer.Deserialize<FantasyPlayer>(doc)!;
            players.Add(p);
        }
        activity?.SetTag("query", query);
        _logger.LogInformation($"Found {players.Count} players");
        activity?.SetTag("player_count", players?.Count);

        return players;
    }

    [HttpGet]
    public IEnumerable<FantasyPlayer> GetPlayers()
    {
        using var activity = _activitySource.StartActivity("Get All Fantasy Players");
        List<FantasyPlayer> players = new List<FantasyPlayer>();

        var result = _ft.Search("idxPlayers", new Query($"*").Limit(0,1700));
        if (result == null)
        {
            _logger.LogError("No players found");
            activity?.SetTag("player_count", 0);
            return players;
        }
        foreach (var doc in result.ToJson())
        {
            FantasyPlayer p = JsonSerializer.Deserialize<FantasyPlayer>(doc)!;
            players.Add(p);
        }
        _logger.LogInformation($"Found {players?.Count} players");
        activity?.SetTag("player_count", players?.Count);

        return players;
    }

    [HttpGet("myplayers")]
    public IEnumerable<FantasyPlayer> GetMyPlayers()
    {
        using var activity = _activitySource.StartActivity("Get My Fantasy Players");
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
        activity?.SetTag("my_player_count", players?.Count);

        return players;
    }

    [HttpPost("assign/{id}")]
    public IActionResult AssignPlayer(string id)
    {
        using var activity = _activitySource.StartActivity("Assign Player");
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        activity?.SetTag("player_id", id);
        if(player == null)
        {
            _logger.LogError($"Player {id} not found.");
            activity?.SetTag("player_found", false);
            return BadRequest();
        }

        activity?.SetTag("player_found", true);
        _logger.LogInformation($"Assigning player {player.FullName} to someone elses team.");

        player.IsOnMyTeam = false;
        player.IsTaken = true;

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
        {
            activity?.SetTag("saved_to_redis", true);
            return Ok();
        }
        else
        {
            activity?.SetTag("saved_to_redis", false);
            return BadRequest();
        }
    }

    [HttpPost("claim/{id}")]
    public IActionResult ClaimPlayer(string id)
    {
        using var activity = _activitySource.StartActivity("Claim Player");
        IDatabase db = _connectionMultiplexer.GetDatabase();
        JsonCommands json = db.JSON();
        FantasyPlayer player = json.Get<FantasyPlayer>($"player:{id}")! ?? null;

        activity?.SetTag("player_id", id);
        if (player == null)
        {
            _logger.LogError($"Player {id} not found.");
            activity?.SetTag("player_found", false);
            return BadRequest();
        }

        activity?.SetTag("player_found", true);
        _logger.LogInformation($"Claiming player {player.FullName} for my team.");

        player.IsOnMyTeam = true;
        player.IsTaken = false;

        bool result = json.Set($"player:{player.SleeperId}", "$", JsonSerializer.Serialize(player));
        _connectionMultiplexer.GetDatabase().KeyExpire($"player:{player.SleeperId}", DateTime.Now.AddDays(1));

        if(result)
        {
            activity?.SetTag("saved_to_redis", true);
            return Ok();
        }
        else
        {
            activity?.SetTag("saved_to_redis", false);
            return BadRequest();
        }
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