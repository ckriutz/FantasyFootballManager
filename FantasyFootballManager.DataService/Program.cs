
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.OM;

using Redis.OM.Contracts;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");
var redisOmConnectionString = Environment.GetEnvironmentVariable("redisOMConnectionString");

Console.WriteLine($"sqlConnectionString: {sqlConnectionString}");
Console.WriteLine($"redisOmConnectionString: {redisOmConnectionString}");

builder.Services.AddDbContext<FantasyFootballManager.DataService.Models.FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient );
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperDraftWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SportsDataIoPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.FantasyProsPlayerWorker>();
builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(redisOmConnectionString));
builder.Services.AddLogging(configure => configure.AddConsole());

var provider = new RedisConnectionProvider(redisOmConnectionString);
provider.Connection.CreateIndex(typeof(FantasyFootballManager.DataService.Models.FantasyPlayer));

IHost host = builder.Build();
host.Run();