using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FantasyFootballManager.DataService;

public sealed class SleeperPlayersWorker
{
    private readonly ILogger<SleeperPlayersWorker> _logger;
    private readonly Models.FantasyDbContext _context;

   public SleeperPlayersWorker(ILogger<SleeperPlayersWorker> logger, Models.FantasyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running SleeperPlayersWorker...");

        await CleanUpDatabaseAsync(cancellationToken);

        Dictionary<string, Models.SleeperPlayer> sleeperPlayersDictionary = new();
        
        try
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            var stringResponse = await client.GetStringAsync("https://api.sleeper.app/v1/players/nfl", cancellationToken);

            sleeperPlayersDictionary = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Models.SleeperPlayer>>(stringResponse);
            if (sleeperPlayersDictionary == null)
            {
                _logger.LogError("Failed to deserialize Sleeper API response.");
                return;
            }
            _logger.LogInformation($"Sleeper API returned {sleeperPlayersDictionary.Count} players.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting data from Sleeper API: {ex.Message}");
        }

        if (sleeperPlayersDictionary.Count == 0)
        {
            _logger.LogError("No data from Sleeper API.");
            return;
        }

        foreach (var player in sleeperPlayersDictionary)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!int.TryParse(player.Key, out _))
                continue;

            await AddPlayerToDatabaseAsync(player.Value, cancellationToken);
        }

        // Update DataStatus once after all players are processed
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "Sleeper", cancellationToken);
        if (ds != null)
        {
            ds.LastUpdated = DateTime.Now.ToLocalTime();
            _context.DataStatus.Update(ds);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Done with data update.");
    }

    private async Task CleanUpDatabaseAsync(CancellationToken cancellationToken)
    {
        var players = await _context.SleeperPlayers
            .Where(p => p.FullName == "Duplicate Player")
            .ToListAsync(cancellationToken);

        foreach (var player in players)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger.LogInformation($"Removing player: {player.PlayerId}");
            _context.SleeperPlayers.Remove(player);
        }
    }

    private async Task<Models.SleeperPlayer?> AddPlayerToDatabaseAsync(Models.SleeperPlayer sleeperPlayer, CancellationToken cancellationToken)
    {
        if (sleeperPlayer.FullName == "Duplicate Player")
            return null;

        var existingPlayer = await _context.SleeperPlayers
            .Include("Team")
            .FirstOrDefaultAsync(p => p.PlayerId == sleeperPlayer.PlayerId, cancellationToken);

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
                existingPlayer.Team = await _context.Teams.FirstOrDefaultAsync(t => t.Abbreviation == sleeperPlayer.TeamAbbreviation, cancellationToken);
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

            _context.SleeperPlayers.Update(existingPlayer);

            return existingPlayer;
        }
        _logger.LogInformation($"Adding new player: {sleeperPlayer.SearchFullName}");

        if (!String.IsNullOrEmpty(sleeperPlayer.TeamAbbreviation))
        {       
            sleeperPlayer.Team = _context.Teams.FirstOrDefault(t => t.Abbreviation == sleeperPlayer.TeamAbbreviation);
        }
 
        _context.SleeperPlayers.Add(sleeperPlayer);

        return sleeperPlayer;
    }


    
}