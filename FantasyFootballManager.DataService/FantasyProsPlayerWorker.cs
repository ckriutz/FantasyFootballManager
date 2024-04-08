using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using FantasyFootballManager.DataService.Models;
using Microsoft.IdentityModel.Tokens;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;

namespace FantasyFootballManager.DataService;

public sealed class FantasyProsPlayerWorker : BackgroundService
{
    private readonly ILogger<FantasyProsPlayerWorker> _logger;
    private readonly Models.FantasyDbContext _context;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

   public FantasyProsPlayerWorker(ILogger<FantasyProsPlayerWorker> logger, Models.FantasyDbContext context, IConnectionMultiplexer multiplexer)
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

            //First, lets check the Sleeper API, and get the newest player data.
            
            //This simulates (for now) the call to the api.
            //string fileName = "Models/FantasyProsPlayers.json";
            //string jsonString = File.ReadAllText(fileName);
            
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("fantasyProsXApiKey"));
            var jsonString = await client.GetStringAsync("https://api.fantasypros.com/public/v2/json/nfl/2024/consensus-rankings?position=ALL&week=0");
            
            var fantasyProsPlayers = JsonSerializer.Deserialize<Models.FantasyProsReturnObject>(jsonString)!;
            _logger.LogInformation($"Found {fantasyProsPlayers.Players.Count()} players from FantasyPros.");

            foreach (var player in fantasyProsPlayers.Players)
            {
                await AddPlayerToDatabaseAsync(player);
                
            }

            // Okay, now that this is done, we need to add the updated date to the database.
            var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "FantasyPros");
            ds.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Done with data update. Going to wait for 1 day.");
        }

    }

    private async Task<Models.FantasyProsPlayer> AddPlayerToDatabaseAsync(Models.FantasyProsPlayer prosPlayer)
    {
        _logger.LogInformation($"Adding player {prosPlayer.PlayerName} to database.");

        // first lets see if we have this player in the database already.
        var existingPlayer = await _context.FantasyProsPlayers.Include("Team").FirstOrDefaultAsync(p => p.PlayerId == prosPlayer.PlayerId);

        if(existingPlayer != null)
        {
            // plate already exists, so lets update it.
            existingPlayer.PlayerName = prosPlayer.PlayerName;
            existingPlayer.SportsdataId = prosPlayer.SportsdataId;

            // OKay, do players who aer 'Inactive' probably don't have a team, so we need to see if it's null first.
            if (!String.IsNullOrEmpty(prosPlayer.PlayerTeamId))
            {
                // You know what? Who cares what the existing team is. Just update it.
                if(prosPlayer.PlayerTeamId == "JAC")
                {
                    prosPlayer.PlayerTeamId = "JAX";
                }
                
                existingPlayer.Team = _context.Teams.FirstOrDefault(t => t.Abbreviation == prosPlayer.PlayerTeamId)!;
            }
            else
            {
                existingPlayer.Team = null;
            }
            existingPlayer.PlayerPositionId = prosPlayer.PlayerPositionId;
            existingPlayer.PlayerPositions = prosPlayer.PlayerPositions;
            existingPlayer.PlayerEligibility = prosPlayer.PlayerEligibility;
            existingPlayer.PlayerShortName = prosPlayer.PlayerShortName;
            existingPlayer.PlayerYahooPositions = prosPlayer.PlayerYahooPositions;
            existingPlayer.PlayerPageUrl = prosPlayer.PlayerPageUrl;
            existingPlayer.PlayerFilename = prosPlayer.PlayerFilename;
            existingPlayer.PlayerSquareImageUrl = prosPlayer.PlayerSquareImageUrl;
            existingPlayer.PlayerImageUrl = prosPlayer.PlayerImageUrl;
            existingPlayer.PlayerYahooId = prosPlayer.PlayerYahooId;
            existingPlayer.CbsPlayerId = prosPlayer.CbsPlayerId;
            existingPlayer.PlayerByeWeek = prosPlayer.PlayerByeWeek;
            existingPlayer.PlayerOwnedAvg = prosPlayer.PlayerOwnedAvg;
            existingPlayer.PlayerOwnedEspn = prosPlayer.PlayerOwnedEspn;
            existingPlayer.PlayerOwnedYahoo = prosPlayer.PlayerOwnedYahoo;
            existingPlayer.RankEcr = prosPlayer.RankEcr;
            existingPlayer.RankMin = prosPlayer.RankMin;
            existingPlayer.RankMax = prosPlayer.RankMax;
            existingPlayer.RankAve = prosPlayer.RankAve;
            existingPlayer.RankStd = prosPlayer.RankStd;
            existingPlayer.PosRank = prosPlayer.PosRank;
            existingPlayer.Tier = prosPlayer.Tier;
            existingPlayer.LastUpdated = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();

                await AddFantasyProsPlayerToRedis(prosPlayer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating player {prosPlayer.PlayerName} in database. {ex.Message}");
            }
           
            
            return existingPlayer;
            
        }
        else
        {
            // player doesn't exist, so lets add it.
            _logger.LogInformation($"Adding player {prosPlayer.PlayerName} to database.");

            if (!String.IsNullOrEmpty(prosPlayer.PlayerTeamId))
            {
                if(prosPlayer.PlayerTeamId == "JAC")
                {
                    prosPlayer.PlayerTeamId = "JAX";
                }
                prosPlayer.Team = _context.Teams.FirstOrDefault(t => t.Abbreviation == prosPlayer.PlayerTeamId)!;
                prosPlayer.LastUpdated = DateTime.Now.ToLocalTime();
            }

            try
            {
                await _context.FantasyProsPlayers.AddAsync(prosPlayer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding player {prosPlayer.PlayerName} to database. {ex.Message}");
            }

            await AddFantasyProsPlayerToRedis(prosPlayer);

            return prosPlayer;
        }
    
    }

    private async Task<DateTime> GetLastUpdatedTime()
    {
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "FantasyPros");
        return ds.LastUpdated.ToLocalTime();
    }

    private async Task AddFantasyProsPlayerToRedis(FantasyProsPlayer player)
    {
        //check to see if we find a player in Redis based on the sportsdata_id field.
        JsonCommands json = _connectionMultiplexer.GetDatabase().JSON();
        SearchCommands ft = _connectionMultiplexer.GetDatabase().FT();

        Models.FantasyPlayer fantasyPlayer = null;

        try 
        {

            // First lets see if we can find the player by it's SportRadarId id.
            var redisPlayerBySportsDataId = ft.Search("idxPlayers", new Query($"@SportRadarId:{player.SportsdataId}")).ToJson().FirstOrDefault();
            if(redisPlayerBySportsDataId != null)
            {
                _logger.LogInformation($"Found existing player in Redis by it's Key! Updating.");

                //There is, so lets deserialize the object to use.
                fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(redisPlayerBySportsDataId)!;
                fantasyPlayer.UpdatePlayerWithProsData(player);

                json.Set($"player:{fantasyPlayer.SleeperId}", "$", JsonSerializer.Serialize(fantasyPlayer));
                _connectionMultiplexer.GetDatabase().KeyExpire($"player:{fantasyPlayer.SleeperId}", DateTime.Now.AddDays(1));
                return;
            }

            // Now lets try and find the player by it's yahooid.
            var redisPlayerByYahooId = ft.Search("idxPlayers", new Query($"@YahooId:{player.PlayerYahooId}")).ToJson().FirstOrDefault();
            if(redisPlayerByYahooId != null)
            {
                //There is, so lets deserialize the object to use.
                fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(redisPlayerByYahooId)!;
                fantasyPlayer.UpdatePlayerWithProsData(player);

                json.Set($"player:{fantasyPlayer.SleeperId}", "$", JsonSerializer.Serialize(fantasyPlayer));
                _connectionMultiplexer.GetDatabase().KeyExpire($"player:{fantasyPlayer.SleeperId}", DateTime.Now.AddDays(1));
                return;
            }

            // Last resort, lets see if we can find them by nane.
            var redisPlayerByName= ft.Search("idxPlayers", new Query($"@FullName:{player.PlayerName}")).ToJson().FirstOrDefault();
            if(redisPlayerByName != null)
            {
                //There is, so lets deserialize the object to use.
                fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(redisPlayerByName)!;
                fantasyPlayer.UpdatePlayerWithProsData(player);

                json.Set($"player:{fantasyPlayer.SleeperId}", "$", JsonSerializer.Serialize(fantasyPlayer));
                _connectionMultiplexer.GetDatabase().KeyExpire($"player:{fantasyPlayer.SleeperId}", DateTime.Now.AddDays(1));
                return;
            }

            _logger.LogWarning($"Could not find player {player.PlayerName} in Redis.");
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding player {player.PlayerName} to Redis. {ex.Message}");
            return;
        }
    }
}