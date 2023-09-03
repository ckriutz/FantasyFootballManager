using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");
var redisConnectionString = Environment.GetEnvironmentVariable("redisConnectionString");

builder.Services.AddDbContext<FantasyFootballManager.DataService.Models.FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient );
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SportsDataIoPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.FantasyProsPlayerWorker>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddLogging(configure => configure.AddConsole());

IHost host = builder.Build();
host.Run();