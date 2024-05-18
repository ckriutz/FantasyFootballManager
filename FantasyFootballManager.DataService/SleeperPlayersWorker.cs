using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Contracts;

namespace FantasyFootballManager.DataService;

public sealed class SleeperPlayersWorker : BackgroundService
{
    private readonly ILogger<SleeperPlayersWorker> _logger;
    private readonly Models.FantasyDbContext _context;
    private readonly IRedisConnectionProvider _connectionProvider;

   public SleeperPlayersWorker(ILogger<SleeperPlayersWorker> logger, Models.FantasyDbContext context, IRedisConnectionProvider connectionProvider)
    {
        _logger = logger;
        _context = context;
        _connectionProvider = connectionProvider;
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

            //await CleanUpDatabase();

            var lastUpdate = await GetLastUpdatedTime();
            if (lastUpdate.AddDays(1) > DateTime.Now)
            {
                _logger.LogInformation($"Data was updated less than 1 day ago. Exiting. Will update again around {lastUpdate.AddDays(1)}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }
            
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

            var sleeperPlayersDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Models.SleeperPlayer>>(jsonString)!;

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

    private async Task CleanUpDatabase()
    {
        // So we need to go though all the players in the database, and see if their Full Name is "Duplicate Player".
        // If it is, we need to remove it from both SQL AND Redis.
        var redisPlayers = _connectionProvider.RedisCollection<Models.FantasyPlayer>();
        var players = await _context.SleeperPlayers.Where(p => p.FullName == "Duplicate Player").ToListAsync();
        foreach (var player in players)
        {
            _logger.LogInformation($"Removing player: {player.PlayerId}");
            _context.SleeperPlayers.Remove(player);
            await _context.SaveChangesAsync();
            var redisPlayer = redisPlayers.Where(p => p.SleeperId == player.PlayerId).FirstOrDefault();
            if (redisPlayer != null)
            {
                await redisPlayers.DeleteAsync(redisPlayer);
            }
        }
    }
    private async Task<Models.SleeperPlayer> AddPlayerToDatabaseAsync(Models.SleeperPlayer sleeperPlayer)
    {
        // So there can be some players named "Duplicate Player".
        // We need to skip those.
        if (sleeperPlayer.FullName == "Duplicate Player")
        {
            return null;
        }

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
            await AddSleeperPlayerToRedisORM(sleeperPlayer);


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
        await AddSleeperPlayerToRedisORM(sleeperPlayer);

        return sleeperPlayer;
    }

    private async Task<DateTime> GetLastUpdatedTime()
    {
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "Sleeper");
        return ds.LastUpdated.ToLocalTime();
    }

    private async Task AddSleeperPlayerToRedisORM(Models.SleeperPlayer player)
    {
        // In this case, I think we're going to insert every player, even the ones not worthy.
        // The API can filter those out if we want.
        var players = _connectionProvider.RedisCollection<Models.FantasyPlayer>();
        var existingPlayer = players.Where(p => p.SleeperId == player.PlayerId).FirstOrDefault();
        if(existingPlayer != null)
        {
            // There is an existing player in Redis. Lets update it.
            _logger.LogInformation($"Player {player.FullName} - {player.PlayerId} is in Redis. Updating.");
            existingPlayer.UpdatePlayerWithSleeperData(player);
            if (!String.IsNullOrEmpty(player.TeamAbbreviation))
            {
                // Okay, so we have one, we just need update the player with this.
                // Both the Team Name, and the Team ID.
                existingPlayer.TeamName = _context.Teams.FirstOrDefault(t => t.Abbreviation == player.TeamAbbreviation)?.Name;
                existingPlayer.TeamId = _context.Teams.FirstOrDefault(t => t.Abbreviation == player.TeamAbbreviation)?.Id;
            }
            await players.UpdateAsync(existingPlayer);
        }
        else
        {
            // This is a new player. Lets add it.
            _logger.LogInformation($"Player {player.FullName} - {player.PlayerId} is not in Redis. Adding.");
            Models.FantasyPlayer fantasyPlayer = new Models.FantasyPlayer();
            if (!String.IsNullOrEmpty(player.TeamAbbreviation))
            {
                fantasyPlayer.TeamName = _context.Teams.FirstOrDefault(t => t.Id == player.Team.Id)?.Name;
                fantasyPlayer.TeamId = _context.Teams.FirstOrDefault(t => t.Abbreviation == player.TeamAbbreviation)?.Id;
            }
            fantasyPlayer.UpdatePlayerWithSleeperData(player);
            await players.InsertAsync(fantasyPlayer);
        }
    }
}