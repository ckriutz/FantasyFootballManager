using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.DataService;

public sealed class SleeperDraftWorker : BackgroundService
{
    private readonly ILogger<SleeperDraftWorker> _logger;
    private readonly Models.FantasyDbContext _context;

    private readonly string mySleeperUserId = Environment.GetEnvironmentVariable("mySleeperId");
    private readonly string sleeperLeaugeId = Environment.GetEnvironmentVariable("leagueId");
   public SleeperDraftWorker(ILogger<SleeperDraftWorker> logger, Models.FantasyDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(DateTime.Now > new DateTime(2023, 9, 5))
        {
            _logger.LogInformation("It's after 9/5/2023. Sleeper Draft is over. Exiting.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if(stoppingToken.IsCancellationRequested)
            {
                _logger.LogError("Cancellation requested. Exiting.");
                return;
            }

            var lastUpdate = await GetLastUpdatedTime();

            _logger.LogInformation($"Last update was {lastUpdate.ToLocalTime()}.");
            _logger.LogInformation($"Current time is {DateTime.Now}.");

            if (lastUpdate.AddMinutes(5) > DateTime.Now)
            {
                _logger.LogInformation($"Data was updated less than 5mins ago. Exiting. Will update again around {lastUpdate.AddMinutes(5)}");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                continue;
            }

            _logger.LogInformation("Updating Sleeper Draft Info");

            var jsonString = string.Empty;
            try
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();
                jsonString = await client.GetStringAsync($"https://api.sleeper.app/v1/draft/{sleeperLeaugeId}/picks");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting data from Sleeper API: {ex.Message}");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                continue;
            }
            var sleeperDraftResults = JsonSerializer.Deserialize<List<Models.SleeperDraftResult>>(jsonString)!;

            foreach(Models.SleeperDraftResult player in sleeperDraftResults)
            {
                Models.FantasyPlayer fantasyPlayer = new();

                try
                {
                    // Lets look up the player in the FantasyPlayer table.
                    var fPlayer = _context.FantasyPlayers.FirstOrDefault(p => p.PlayerId == player.PlayerId);
                    if(fPlayer != null)
                    {
                        _logger.LogInformation($"Updating status of {player.PlayerId}");
                        fPlayer.PickedBy = player.PickedBy;
                        fPlayer.PickNumber = player.PickNo;
                        fPlayer.PickRound = player.Round;

                        // Now we set the IsOnMyTeam flag.
                        if(player.PickedBy == mySleeperUserId)
                        {
                            fantasyPlayer.IsOnMyTeam = true;
                            fantasyPlayer.IsTaken = false;
                        }
                        else
                        {
                            fantasyPlayer.IsOnMyTeam = false;
                            fantasyPlayer.IsTaken = true;
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something shit the bed trying to work with {player.PlayerId} in the database");
                }

            }

            // Okay, now that this is done, we need to add the updated date to the database.
            var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "SleeperDraft");
            ds.LastUpdated = DateTime.Now.ToLocalTime();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Done with data update. Going to wait for 5 minutes.");  

        }
    }

    private async Task<DateTime> GetLastUpdatedTime()
    {
        var ds = await _context.DataStatus.FirstOrDefaultAsync(d => d.DataSource == "SleeperDraft");
        return ds.LastUpdated.ToLocalTime();
    }
}