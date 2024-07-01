using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");

Console.WriteLine($"sqlConnectionString: {sqlConnectionString}");

builder.Services.AddDbContext<FantasyFootballManager.DataService.Models.FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient );
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperDraftWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SportsDataIoPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.FantasyProsPlayerWorker>();
builder.Services.AddLogging(configure => configure.AddConsole());

IHost host = builder.Build();
host.Run();
