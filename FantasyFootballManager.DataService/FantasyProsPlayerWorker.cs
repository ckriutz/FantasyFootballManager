using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.DataService;

public sealed class FantasyProsPlayerWorker : BackgroundService
{
    private readonly ILogger<FantasyProsPlayerWorker> _logger;
    private readonly Models.FantasyDbContext _context;

   public FantasyProsPlayerWorker(ILogger<FantasyProsPlayerWorker> logger, Models.FantasyDbContext context)
    {
        _logger = logger;
        _context = context;
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

            var jsonString = string.Empty;
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("fantasyProsXApiKey"));
                jsonString = await client.GetStringAsync("https://api.fantasypros.com/public/v2/json/nfl/2024/consensus-rankings?position=ALL&week=0");

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from FantasyPros: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }
            
            var fantasyProsPlayers = JsonSerializer.Deserialize<Models.FantasyProsReturnObject>(jsonString)!;

            _logger.LogInformation($"Found {fantasyProsPlayers.Players.Count()} players from FantasyPros.");

            foreach (var player in fantasyProsPlayers.Players)
            {
                if( player.SportsdataId == null)
                {
                    // Some players, I don't know why yet, don't have a sportsdata_id. I don't want to make the column nullable.
                    _logger.LogWarning($"Player {player.PlayerName} does not have a SportsDataId. Skipping.");
                    continue;
                }
                await AddPlayerToDatabaseAsync(player);
                
            }

            // Okay, now that this is done, we need to add the updated date to the database.
            var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "FantasyPros");
            ds.LastUpdated = DateTime.Now;
            _context.DataStatus.Update(ds);
            _context.SaveChanges();

            _logger.LogInformation("Done with data update. Going to wait for 1 day.");
            return;
        }

    }

    private async Task<Models.FantasyProsPlayer> AddPlayerToDatabaseAsync(Models.FantasyProsPlayer prosPlayer)
    {
        // first lets see if we have this player in the database already.
        var existingPlayer = await _context.FantasyProsPlayers.Include("Team").FirstOrDefaultAsync(p => p.PlayerId == prosPlayer.PlayerId);

        if(existingPlayer != null)
        {
            _logger.LogInformation($"Updating player {prosPlayer.PlayerName} in database.");
            // player already exists, so lets update it.
            existingPlayer.PlayerName = prosPlayer.PlayerName;
            existingPlayer.SportsdataId = prosPlayer.SportsdataId;

            // Okay, do players who are 'Inactive' probably don't have a team, so we need to see if it's null first.
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
            existingPlayer.PlayerYahooPositions = prosPlayer.PlayerYahooPositions ?? "UNK";
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
            existingPlayer.LastUpdated = DateTime.Now.ToLocalTime();

            try
            {
                _context.FantasyProsPlayers.Update(existingPlayer);
                _context.SaveChanges();
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
                // Quick check to see if PlayerYahooPositions is null. If it is, set it to UNK.
                prosPlayer.PlayerYahooPositions = String.IsNullOrEmpty(prosPlayer.PlayerYahooPositions) ? prosPlayer.PlayerYahooPositions = "UNK" : prosPlayer.PlayerYahooPositions;
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

            return prosPlayer;
        }
    
    }

    private async Task<DateTime> GetLastUpdatedTime()
    {
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "FantasyPros");
        return ds.LastUpdated.ToLocalTime();
    }

}