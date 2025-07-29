using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.DataService;

public class WorkerSchedulerService : BackgroundService
{
    private readonly ILogger<WorkerSchedulerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Run every 1 hour

    public WorkerSchedulerService(ILogger<WorkerSchedulerService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunWorkersIfNeeded(stoppingToken);
                
                _logger.LogInformation("Waiting {Hours} hours before next check", _interval.TotalHours);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during scheduled execution");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retrying
            }
        }
    }

    private async Task RunWorkersIfNeeded(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Models.FantasyDbContext>();
        
        // Check and run each worker based on last update time
        if (await ShouldRunWorker(context, "Sleeper", cancellationToken))
        {
            var worker = scope.ServiceProvider.GetRequiredService<SleeperPlayersWorker>();
            await worker.RunAsync(cancellationToken);
        }

        if (await ShouldRunWorker(context, "SportsDataIO", cancellationToken))
        {
            var worker = scope.ServiceProvider.GetRequiredService<SportsDataIoPlayersWorker>();
            await worker.RunAsync(cancellationToken);
        }

        if (await ShouldRunWorker(context, "FantasyPros", cancellationToken))
        {
            var worker = scope.ServiceProvider.GetRequiredService<FantasyProsPlayerWorker>();
            await worker.RunAsync(cancellationToken);
        }
    }

    private async Task<bool> ShouldRunWorker(Models.FantasyDbContext context, string dataSource, CancellationToken cancellationToken)
    {
        var dataStatus = await context.DataStatus
            .FirstOrDefaultAsync(d => d.DataSource == dataSource, cancellationToken);
        
        if (dataStatus == null) return true;
        
        var hoursSinceLastUpdate = (DateTime.UtcNow - dataStatus.LastUpdated).TotalHours;

        if (hoursSinceLastUpdate < 24)
        {
            _logger.LogInformation("Worker {DataSource} skipped. Last updated {Hours:F1} hours ago.", dataSource, hoursSinceLastUpdate);
            return false;
        }
        _logger.LogInformation("Worker {DataSource} will run. Last updated {Hours:F1} hours ago.", dataSource, hoursSinceLastUpdate);
        return true;
    }
}