using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FantasyFootballManager.DataService.Models;
using FantasyFootballManager.DataService;

var postgresConnectionString = Environment.GetEnvironmentVariable("postgresConnectionString");
if (string.IsNullOrWhiteSpace(postgresConnectionString))
{
    Console.WriteLine("ERROR: postgresConnectionString environment variable is not set.");
    return;
}

Console.WriteLine($"Starting Data Service version 1.3.0");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<FantasyDbContext>(options =>
            options.UseNpgsql(postgresConnectionString), ServiceLifetime.Transient);

        services.AddLogging(configure => configure.AddConsole());

        // Register workers as transient services
        services.AddTransient<SleeperPlayersWorker>();
        services.AddTransient<SportsDataIoPlayersWorker>();
        services.AddTransient<FantasyProsPlayerWorker>();

        // Register the worker scheduler service
        services.AddHostedService<WorkerSchedulerService>();
    })
    .Build();

await host.RunAsync();