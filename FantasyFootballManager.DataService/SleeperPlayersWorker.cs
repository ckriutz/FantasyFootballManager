using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using System.Text.Json;


namespace FantasyFootballManager.DataService;

public sealed class SleeperPlayersWorker : BackgroundService
{
    private readonly ILogger<SleeperPlayersWorker> _logger;
    private readonly Models.FantasyDbContext _context;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

   public SleeperPlayersWorker(ILogger<SleeperPlayersWorker> logger, Models.FantasyDbContext context, IConnectionMultiplexer multiplexer)
    {
        _logger = logger;
        _context = context;
        _connectionMultiplexer = multiplexer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Sleeper Players Worker...");

        while (!stoppingToken.IsCancellationRequested)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Exiting.");
                return;
            }

            var lastUpdate = await GetLastUpdatedTime();
            if (lastUpdate.AddDays(1) > DateTime.Now)
            {
                _logger.LogInformation($"Data was updated less than 1 day ago. Exiting. Will update again around {lastUpdate.AddDays(1)}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }

            // First, lets check the Sleeper API, and get the newest player data.
            // This simulates (for now) the call to the api.
           // string fileName = "Models/SleeperPlayersSmall.json";
            //string jsonString = File.ReadAllText(fileName);
            
            var jsonString = string.Empty;
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                jsonString = await client.GetStringAsync("https://api.sleeper.app/v1/players/nfl");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from Sleeper API: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }

            var sleeperPlayersDictionary = JsonSerializer.Deserialize<Dictionary<string, Models.SleeperPlayer>>(jsonString)!;

            _logger.LogInformation($"Sleeper API returned {sleeperPlayersDictionary.Count} players.");

            //Now, we need to iterate though the players, and add them to the database.
            foreach (var player in sleeperPlayersDictionary)
            {
                // Now, this might be a TEAM and not a player, and if that's the case, we should skip it.
                if (int.TryParse(player.Key, out int result) == false)
                {
                    continue;
                }
                
                await AddPlayerToDatabaseAsync(player.Value);
            }

            // Okay, now that this is done, we need to add the updated date to the database.
            var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "Sleeper");
            ds.LastUpdated = DateTime.Now.ToLocalTime();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Done with data update. Going to wait for 1 day.");  
        }
    }

    private async Task<Models.SleeperPlayer> AddPlayerToDatabaseAsync(Models.SleeperPlayer sleeperPlayer)
    {
        // Does the player already exist?
        var existingPlayer = await _context.SleeperPlayers.Include("Team").FirstOrDefaultAsync(p => p.PlayerId == sleeperPlayer.PlayerId);
        if (existingPlayer != null)
        {
            _logger.LogInformation($"Updating existing player: {sleeperPlayer.SearchFullName}");
            existingPlayer.Status = sleeperPlayer.Status;
            existingPlayer.Position = sleeperPlayer.Position;
            existingPlayer.Number = sleeperPlayer.Number;
            existingPlayer.YahooId = sleeperPlayer.YahooId;
            existingPlayer.StatsId = sleeperPlayer.StatsId;
            existingPlayer.InjuryBodyPart = sleeperPlayer.InjuryBodyPart;
            existingPlayer.FantasyDataId = sleeperPlayer.FantasyDataId;
            existingPlayer.YearsExp = sleeperPlayer.YearsExp;
            existingPlayer.RotoworldId = sleeperPlayer.RotoworldId;
            existingPlayer.GsisId = sleeperPlayer.GsisId;
            existingPlayer.NewsUpdated = sleeperPlayer.NewsUpdated;
            existingPlayer.DepthChartPosition = sleeperPlayer.DepthChartPosition;
            existingPlayer.DepthChartOrder = sleeperPlayer.DepthChartOrder;
            existingPlayer.EspnId = sleeperPlayer.EspnId;
            existingPlayer.Hashtag = sleeperPlayer.Hashtag;
            existingPlayer.Age = sleeperPlayer.Age;
            existingPlayer.College = sleeperPlayer.College;

            // OKay, do players who are 'Inactive' probably don't have a team, so we need to see if it's null first.
            if (!String.IsNullOrEmpty(sleeperPlayer.TeamAbbreviation))
            {
                // You know what? Who cares what the existing team is. Just update it.
                existingPlayer.Team = _context.Teams.FirstOrDefault(t => t.Abbreviation == sleeperPlayer.TeamAbbreviation);
            }
            else
            {
                existingPlayer.Team = null;
            }


            existingPlayer.SearchRank = sleeperPlayer.SearchRank;
            existingPlayer.injuryStartDate = sleeperPlayer.injuryStartDate;
            existingPlayer.InjuryStatus = sleeperPlayer.InjuryStatus;
            existingPlayer.IsActive = sleeperPlayer.IsActive;
            existingPlayer.SwishId = sleeperPlayer.SwishId;
            existingPlayer.InjuryNotes  = sleeperPlayer.InjuryNotes;
            existingPlayer.SportRadarId = sleeperPlayer.SportRadarId;
            existingPlayer.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();

            // Lets publish this player into Redis.
            await AddSleeperPlayerToRedis(existingPlayer);


            return existingPlayer;
        }
        _logger.LogInformation($"Adding new player: {sleeperPlayer.SearchFullName}");

        if (!String.IsNullOrEmpty(sleeperPlayer.TeamAbbreviation))
        {       
            sleeperPlayer.Team = _context.Teams.FirstOrDefault(t => t.Abbreviation == sleeperPlayer.TeamAbbreviation);
        }
 

        _context.SleeperPlayers.Add(sleeperPlayer);
        await _context.SaveChangesAsync();
        
        // Lets publish this player into Redis.
        await AddSleeperPlayerToRedis(sleeperPlayer);

        return sleeperPlayer;
    }

    private async Task<DateTime> GetLastUpdatedTime()
    {
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "Sleeper");
        return ds.LastUpdated.ToLocalTime();
    }

    private async Task AddSleeperPlayerToRedis(Models.SleeperPlayer player)
    {
        // Short Circuit this if the player is not worthy.
        if(player.SearchRank == null || player.SearchRank >= 9999999)
        {
            _logger.LogInformation($"Player {player.FullName} is not worthy. Skipping.");
            return;
        }

        // Lets create a Fantasy Player we will use or adding to Redis.
        Models.FantasyPlayer fantasyPlayer = new Models.FantasyPlayer();

        try
        {
            // Lets see if the obejct is already in Redis, and if so, lets just update that because other data might be in there.
            // Learned something... we need to see if the data is there before we deserialize.
            JsonCommands json =_connectionMultiplexer.GetDatabase().JSON();
            var data = json.Get($"player:{player.PlayerId}");
            if(!data.IsNull)
            {
                _logger.LogInformation($"Player {player.FullName} is in Redis. Updating.");
                fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(data.ToString())!;
            }

            fantasyPlayer.UpdatePlayerWithSleeperData(player);
            if (!String.IsNullOrEmpty(player.TeamAbbreviation))
            {
                fantasyPlayer.TeamName = _context.Teams.FirstOrDefault(t => t.Id == player.Team.Id)?.Name;
            }
            
            // We have to serialize the updated FantasyPlayer object.
            //string serializedFantasyPlayer = JsonSerializer.Serialize(fantasyPlayer);

            // Now, we have to update the player in Redis.
            // Going to set it so it expires in 7 days.
            bool setResult = json.Set($"player:{fantasyPlayer.SleeperId}", "$", JsonSerializer.Serialize(fantasyPlayer));
            if(setResult == true)
            {
                _logger.LogInformation($"Player {fantasyPlayer.FullName} was added to Redis.");
                _connectionMultiplexer.GetDatabase().KeyExpire($"player:{fantasyPlayer.SleeperId}", DateTime.Now.AddDays(7));
            }
            else
            {
                _logger.LogWarning($"Player {fantasyPlayer.FullName} was not added to Redis.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding player {fantasyPlayer.FullName} to Redis. {ex.Message}");
        }
    }
}