using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
namespace FantasyFootballManager.DataService;

public sealed class SportsDataIoPlayersWorker
{
    private readonly ILogger<SportsDataIoPlayersWorker> _logger;
    private readonly Models.FantasyDbContext _context;

    public SportsDataIoPlayersWorker(ILogger<SportsDataIoPlayersWorker> logger, Models.FantasyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running SportsDataIoPlayersWorker...");

        List<Models.SportsDataIoPlayer> sportsDataIoPlayers = new();

        try
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("SportsDataIoOcpApimKey"));
            var stringResponse = await client.GetStringAsync("https://fly.sportsdata.io/v3/nfl/stats/json/FantasyPlayers", cancellationToken);

            sportsDataIoPlayers = JsonSerializer.Deserialize<List<Models.SportsDataIoPlayer>>(stringResponse);
            if (sportsDataIoPlayers == null)
            {
                _logger.LogError("Failed to deserialize SportsData.io API response.");
                return;
            }
            _logger.LogInformation($"SportsData.io API returned {sportsDataIoPlayers.Count} players.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting data from SportsData.io: {ex.Message}");
            return;
        }

        if (sportsDataIoPlayers.Count == 0)
        {
            _logger.LogError("No data from SportsData.io API.");
            return;
        }

        foreach (var player in sportsDataIoPlayers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await AddPlayerToDatabaseAsync(player, cancellationToken);
        }

        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "SportsDataIO", cancellationToken);
        if (ds != null)
        {
            ds.LastUpdated = DateTime.Now.ToLocalTime();
            _context.DataStatus.Update(ds);
        }
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Done with SportsDataIo Data update.");
    }

    private async Task<Models.SportsDataIoPlayer> AddPlayerToDatabaseAsync(Models.SportsDataIoPlayer ioPlayer, CancellationToken cancellationToken)
    {
        var player = await _context.SportsDataIoPlayers.FirstOrDefaultAsync(p => p.PlayerID == ioPlayer.PlayerID, cancellationToken);
        if (player == null)
        {
            _logger.LogInformation($"Adding player {ioPlayer.Name} in database.");

            if (!string.IsNullOrEmpty(ioPlayer.TeamAbbreviation))
            {
                ioPlayer.PlayerTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Abbreviation == ioPlayer.TeamAbbreviation, cancellationToken);
            }
            ioPlayer.LastUpdated = DateTime.Now.ToLocalTime();

            await _context.SportsDataIoPlayers.AddAsync(ioPlayer, cancellationToken);

            return ioPlayer;
        }
        else
        {
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

            if (!string.IsNullOrEmpty(ioPlayer.TeamAbbreviation))
            {
                player.PlayerTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Abbreviation == ioPlayer.TeamAbbreviation, cancellationToken);
            }
            else
            {
                player.PlayerTeam = null;
            }
            player.LastUpdated = DateTime.Now.ToLocalTime();
            _context.SportsDataIoPlayers.Update(player);

            return player;
        }
    }
}