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

public sealed class SleeperDraftWorker : BackgroundService
{
    private readonly ILogger<SleeperDraftWorker> _logger;
    private readonly Models.FantasyDbContext _context;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    private readonly string mySleeperUserId = Environment.GetEnvironmentVariable("mySleeperId");
    private readonly string sleeperLeaugeId = Environment.GetEnvironmentVariable("leagueId");
   public SleeperDraftWorker(ILogger<SleeperDraftWorker> logger, Models.FantasyDbContext context, IConnectionMultiplexer multiplexer)
    {
        _logger = logger;
        _context = context;
        _connectionMultiplexer = multiplexer;
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

            // First, lets check the Sleeper API, and get the newest player data.
            // This simulates (for now) the call to the api.
            //string fileName = "Models/Examples/SleeperDraft.json";
            //string jsonString = File.ReadAllText(fileName);

            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            var jsonString = await client.GetStringAsync($"https://api.sleeper.app/v1/draft/{sleeperLeaugeId}/picks");

            var sleeperDraftResults = JsonSerializer.Deserialize<List<Models.SleeperDraftResult>>(jsonString)!;

            foreach(Models.SleeperDraftResult player in sleeperDraftResults)
            {
                Models.FantasyPlayer fantasyPlayer = new();

                try
                {
                    JsonCommands json =_connectionMultiplexer.GetDatabase().JSON();
                    var data = json.Get($"player:{player.PlayerId}");
                    if(!data.IsNull)
                    {
                        _logger.LogInformation($"Player {player.Metadata.FirstName} {player.Metadata.LastName} is in Redis. Updating.");
                        fantasyPlayer = JsonSerializer.Deserialize<Models.FantasyPlayer>(data.ToString())!;
                        fantasyPlayer.PickedBy = player.PickedBy;
                        fantasyPlayer.PickNumber = player.PickNo;
                        fantasyPlayer.PickRound = player.Round;

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

                        bool setResult = json.Set($"player:{fantasyPlayer.SleeperId}", "$", JsonSerializer.Serialize(fantasyPlayer));
                        if(setResult == true)
                        {
                            _logger.LogInformation($"Player {fantasyPlayer.FullName} was updated with draft information.");
                            _connectionMultiplexer.GetDatabase().KeyExpire($"player:{fantasyPlayer.SleeperId}", DateTime.Now.AddDays(1));
                        }
                        else
                        {
                            _logger.LogWarning($"Player {fantasyPlayer.FullName} was not updated in Redis.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Something shit the bed trying to work with {player.PlayerId} in Redis");
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