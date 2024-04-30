using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Redis.OM;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Redis.OM.Contracts;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");
var redisOmConnectionString = Environment.GetEnvironmentVariable("redisOMConnectionString");

builder.Services.AddDbContext<FantasyFootballManager.DataService.Models.FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient );
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SleeperDraftWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.SportsDataIoPlayersWorker>();
builder.Services.AddHostedService<FantasyFootballManager.DataService.FantasyProsPlayerWorker>();
builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(redisOmConnectionString));
builder.Services.AddLogging(configure => configure.AddConsole());

var provider = new Redis.OM.RedisConnectionProvider(redisOmConnectionString);
provider.Connection.CreateIndex(typeof(FantasyFootballManager.DataService.Models.FantasyPlayer));

IHost host = builder.Build();
host.Run();