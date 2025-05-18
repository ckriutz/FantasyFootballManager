using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FantasyFootballManager.DataService.Models;
using FantasyFootballManager.DataService;

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");
if (string.IsNullOrWhiteSpace(sqlConnectionString))
{
    Console.WriteLine("ERROR: sqlConnectionString environment variable is not set.");
    return;
}

Console.WriteLine($"Starting Data Service version 1.2.0");

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<FantasyDbContext>(options =>
            options.UseSqlServer(sqlConnectionString), ServiceLifetime.Transient);

        services.AddLogging(configure => configure.AddConsole());

        // Register workers as transient services
        services.AddTransient<SleeperPlayersWorker>();
        services.AddTransient<SportsDataIoPlayersWorker>();
        services.AddTransient<FantasyProsPlayerWorker>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var cancellationToken = new CancellationTokenSource().Token;

try
{
    var sleeperPlayersWorker = scope.ServiceProvider.GetRequiredService<SleeperPlayersWorker>();
    await sleeperPlayersWorker.RunAsync(cancellationToken);

    var sportsDataIoPlayersWorker = scope.ServiceProvider.GetRequiredService<SportsDataIoPlayersWorker>();
    await sportsDataIoPlayersWorker.RunAsync(cancellationToken);

    var fantasyProsPlayerWorker = scope.ServiceProvider.GetRequiredService<FantasyProsPlayerWorker>();
    await fantasyProsPlayerWorker.RunAsync(cancellationToken);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Unhandled exception occurred during worker execution.");
}
