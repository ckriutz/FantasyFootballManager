using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Azure.Core.Pipeline;

namespace FantasyFootballManager.DataService;

public sealed class FantasyProsPlayerWorker
{
    private readonly ILogger<FantasyProsPlayerWorker> _logger;
    private readonly Models.FantasyDbContext _context;

   public FantasyProsPlayerWorker(ILogger<FantasyProsPlayerWorker> logger, Models.FantasyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running FantasyProsPlayerWorker...");

        Models.FantasyProsReturnObject? fantasyProsPlayers = null;

        try
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("fantasyProsXApiKey"));
            var stringResponse = await client.GetStringAsync("https://api.fantasypros.com/public/v2/json/nfl/2024/consensus-rankings?position=ALL&week=0", cancellationToken);

            fantasyProsPlayers = JsonSerializer.Deserialize<Models.FantasyProsReturnObject>(stringResponse);
            if (fantasyProsPlayers == null)
            {
                _logger.LogError("Failed to deserialize FantasyPros API response.");
                return;
            }
            _logger.LogInformation($"FantasyPros API returned {fantasyProsPlayers.Players.Count()} players.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting data from FantasyPros API: {ex.Message}");
        }

        if (fantasyProsPlayers == null || fantasyProsPlayers.Players.Count() == 0)
        {
            _logger.LogError("No data from FantasyPros API.");
            return;
        }

        int batchSize = 25; // Process in batches to avoid memory issues
        int processedCount = 0;

        foreach (var player in fantasyProsPlayers.Players)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (player.SportsdataId == null)
            {
                _logger.LogWarning($"Player {player.PlayerName} does not have a SportsDataId. Skipping.");
                continue;
            }
            await AddPlayerToDatabaseAsync(player, cancellationToken);
            processedCount++;

            // Save changes in batches
            if (processedCount % batchSize == 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Saved batch of {batchSize} players. Total processed: {processedCount}");
            }
        }

        // Save any remaining changes
        if (processedCount % batchSize != 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Saved final batch. Total players processed: {processedCount}");
        }
        
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "FantasyPros", cancellationToken);
        if (ds == null)
        {
            ds = new Models.DataStatus { DataSource = "FantasyPros" };
            _context.DataStatus.Add(ds);
        }

        ds.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Done with data update.");
        return;
    }

    private async Task<Models.FantasyProsPlayer> AddPlayerToDatabaseAsync(Models.FantasyProsPlayer prosPlayer, CancellationToken cancellationToken)
    {
        var existingPlayer = await _context.FantasyProsPlayers.Include(p => p.Team).FirstOrDefaultAsync(p => p.PlayerId == prosPlayer.PlayerId, cancellationToken);

        if(existingPlayer != null)
        {
            _logger.LogInformation($"Updating player {prosPlayer.PlayerName} in database.");
            existingPlayer.PlayerName = prosPlayer.PlayerName;
            existingPlayer.SportsdataId = prosPlayer.SportsdataId;

            if (!String.IsNullOrEmpty(prosPlayer.PlayerTeamId))
            {
                Console.WriteLine($"Team Abbreviation: {prosPlayer.PlayerTeamId}");
                if (prosPlayer.PlayerTeamId == "JAC")
                    prosPlayer.PlayerTeamId = "JAX";
                existingPlayer.Team = await _context.Teams.FirstOrDefaultAsync(t => t.Abbreviation == prosPlayer.PlayerTeamId, cancellationToken);
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
            existingPlayer.LastUpdated = DateTime.UtcNow;

            try
            {
                _context.FantasyProsPlayers.Update(existingPlayer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating player {prosPlayer.PlayerName} in database. {ex.Message}");
            }
           
            return existingPlayer;
        }
        else
        {
            _logger.LogInformation($"Adding player {prosPlayer.PlayerName} to database.");

            if (!String.IsNullOrEmpty(prosPlayer.PlayerTeamId))
            {
                if(prosPlayer.PlayerTeamId == "JAC")
                    prosPlayer.PlayerTeamId = "JAX";
                prosPlayer.PlayerYahooPositions = String.IsNullOrEmpty(prosPlayer.PlayerYahooPositions) ? "UNK" : prosPlayer.PlayerYahooPositions;
                prosPlayer.Team = await _context.Teams.FirstOrDefaultAsync(t => t.Abbreviation == prosPlayer.PlayerTeamId, cancellationToken);
                prosPlayer.LastUpdated = DateTime.UtcNow;
            }

            try
            {
                _context.FantasyProsPlayers.Add(prosPlayer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding player {prosPlayer.PlayerName} to database. {ex.Message}");
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(prosPlayer.ToString());
                return prosPlayer; // Return the player even if there was an error staging it
            }

            return prosPlayer;
        }
    }
}