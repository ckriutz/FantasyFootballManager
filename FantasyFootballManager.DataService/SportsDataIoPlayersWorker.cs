using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;

namespace FantasyFootballManager.DataService;

public sealed class SportsDataIoPlayersWorker : BackgroundService
{
    private readonly ILogger<SportsDataIoPlayersWorker> _logger;
    private readonly Models.FantasyDbContext _context;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

   public SportsDataIoPlayersWorker(ILogger<SportsDataIoPlayersWorker> logger, Models.FantasyDbContext context, IConnectionMultiplexer multiplexer)
    {
        _logger = logger;
        _context = context;
        _connectionMultiplexer = multiplexer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Exiting.");
                return;
            }

            var lastUpdate = await GetLastUpdatedTime();
            if (lastUpdate.AddHours(12) > DateTime.Now)
            {
                _logger.LogInformation($"Data was updated less than 12 hours ago. Exiting. Will update again around {lastUpdate.AddHours(12)}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }

            _logger.LogInformation("Updating SportsDataIO Players");

            var jsonString = string.Empty;
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("SportsDataIoOcpApimKey"));
                jsonString = await client.GetStringAsync("https://fly.sportsdata.io/v3/nfl/stats/json/FantasyPlayers");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from SportsData.io: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }

            // This simulates (for now) the call to the api.
            //string fileName = "Models/SportsDataIoPlayers.json";
            //string jsonString = File.ReadAllText(fileName);

            var sportsDataIoPlayers = JsonSerializer.Deserialize<List<Models.SportsDataIoPlayer>>(jsonString)!;
            _logger.LogInformation($"Found {sportsDataIoPlayers.Count} players from SportsData.io.");

            foreach (var player in sportsDataIoPlayers)
            {
                await AddPlayerToDatabaseAsync(player);
            }

            // Okay, now that this is done, we need to add the updated date to the database.
            var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "SportsDataIO");
            ds.LastUpdated = DateTime.Now.ToLocalTime();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Done with data update. Going to wait for 12 hours.");
        }
    }

    private async Task<Models.SportsDataIoPlayer> AddPlayerToDatabaseAsync(Models.SportsDataIoPlayer ioPlayer)
    {
        var player = await _context.SportsDataIoPlayers.FirstOrDefaultAsync(p => p.PlayerID == ioPlayer.PlayerID);
        if (player == null)
        {
            _logger.LogInformation($"Adding player {ioPlayer.Name} in database.");

            if (!String.IsNullOrEmpty(ioPlayer.TeamAbbreviation))
            {       
                ioPlayer.PlayerTeam = _context.Teams.FirstOrDefault(t => t.Abbreviation == ioPlayer.TeamAbbreviation);
            }

            await _context.SportsDataIoPlayers.AddAsync(ioPlayer);
            await _context.SaveChangesAsync();

            await AddSportsDataIoPlayerToRedis(player);

            return ioPlayer;
        }
        else
        {
            // lets update the player with the new data.
            _logger.LogInformation($"Player {ioPlayer.Name} already exists in database. Updating.");
            player.Position = ioPlayer.Position;
            player.AverageDraftPosition = ioPlayer.AverageDraftPosition;
            player.AverageDraftPositionPPR = ioPlayer.AverageDraftPositionPPR;
            player.ByeWeek = ioPlayer.ByeWeek;
            player.LastSeasonFantasyPoints = ioPlayer.LastSeasonFantasyPoints;
            player.ProjectedFantasyPoints = ioPlayer.ProjectedFantasyPoints;
            player.AuctionValue = ioPlayer.AuctionValue;
            player.AuctionValuePPR = ioPlayer.AuctionValuePPR;
            player.AverageDraftPositionIDP = ioPlayer.AverageDraftPositionIDP;
            player.AverageDraftPositionRookie = ioPlayer.AverageDraftPositionRookie;
            player.AverageDraftPositionDynasty = ioPlayer.AverageDraftPositionDynasty;
            player.AverageDraftPosition2QB = ioPlayer.AverageDraftPosition2QB;

            // OKay, do players who aer 'Inactive' probably don't have a team, so we need to see if it's null first.
            if (!String.IsNullOrEmpty(ioPlayer.TeamAbbreviation))
            {
                // You know what? Who cares what the existing team is. Just update it.
                player.PlayerTeam = _context.Teams.FirstOrDefault(t => t.Abbreviation == ioPlayer.TeamAbbreviation);
            }
            else
            {
                player.PlayerTeam = null;
            }

            await _context.SaveChangesAsync();

            await AddSportsDataIoPlayerToRedis(player);
            return player;
        }
    }

    private async Task<DateTime> GetLastUpdatedTime()
    {
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "SportsDataIO");
        return ds.LastUpdated.ToLocalTime();
    }

    private string FixedPlayer(string playerName)
    {
        if(playerName == "D.J. Chark")
        {
            return "DJ Chark";
        }
        if(playerName == "Kenneth Walker III")
        {
            return "Kenneth Walker";
        }
        if(playerName == "Mecole Hardman Jr.")
        {
            return "Mecole Hardman";
        }
        return playerName;
    }


    private async Task AddSportsDataIoPlayerToRedis(Models.SportsDataIoPlayer ioPlayer)
    {
        Models.FantasyPlayer fantasyPlayer = null;

        // First, lets hope, hope, hope, that the player is already in Redis.
        JsonCommands json = _connectionMultiplexer.GetDatabase().JSON();
        SearchCommands ft = _connectionMultiplexer.GetDatabase().FT();

        // Some fixes for weird names. I'm looking at you D.J. Chark. Someday there will be a better way to do this.
        ioPlayer.Name = FixedPlayer(ioPlayer.Name);

        // We need to find the played based on only the the data we have.
        var redisPlayerByKey = ft.Search("idxPlayers", new Query($"@SportsDataIoKey:{ioPlayer.FantasyPlayerKey}")).ToJson().FirstOrDefault();
        if(redisPlayerByKey != null)
        {
            _logger.LogInformation($"Found existing player in Redis by it's Key! Updating.");

            //There is, so lets deserialize the object to use.
            fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(redisPlayerByKey)!;
        }
        else
        {
            // Here is the hard part. We need to find the player in Redis by matching their name.
            // We need to do this because the SportsData.io player does not have a unique identifier that matches with anyone really.
            // This will return an list of players, but there should only be one.
            //foreach (var doc in ft.Search("idx1", new Query($"@FullName:{ioPlayer.Name}")).ToJson())
            var redisPlayerByName = ft.Search("idxPlayers", new Query($"@FullName:{ioPlayer.Name}")).ToJson().FirstOrDefault();

            // Is there an object?
            if(redisPlayerByName != null)
            {
                _logger.LogInformation($"Found existing player in Redis by name. Updating.");

                //There is, so lets deserialize the object to use.
                fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(redisPlayerByName)!;
            }

        }
        // Hopefully we found something.
        if(fantasyPlayer != null)
        {
            // We need to update the FantasyPlayer object with the ioPlayer object.
            fantasyPlayer.UpdatePlayerWithIoData(ioPlayer);

            // We have to serialize the updated FantasyPlayer object.
            //string serializedFantasyPlayer = JsonSerializer.Serialize(fantasyPlayer);

            // Now, we have to update the player in Redis.
            json.Set($"player:{fantasyPlayer.SleeperId}", "$", JsonSerializer.Serialize(fantasyPlayer));
            _connectionMultiplexer.GetDatabase().KeyExpire($"player:{fantasyPlayer.SleeperId}", DateTime.Now.AddDays(1));
        }
        else
        {
            _logger.LogWarning($"We didn't find {ioPlayer.Name} in the database, so we're going to skip.");
        }

    }
}