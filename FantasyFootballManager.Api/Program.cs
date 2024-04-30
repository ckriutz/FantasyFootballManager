using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FantasyFootballManager.DataService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.OM;
using Redis.OM.Contracts;


var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");
var redisOMConnectionString = Environment.GetEnvironmentVariable("redisOMConnectionString");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient );
builder.Services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(redisOMConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var provider = app.Services.GetRequiredService<IRedisConnectionProvider>();
var fantasyplayers = provider.RedisCollection<FantasyPlayer>();


app.MapGet("/datastatus", () => 
{ 
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FantasyDbContext>();
    var dataStatus = dbContext.DataStatus.ToArray();
    return dataStatus;

});

// Get a single player by SleeperId using Redis.OM
app.MapGet("/fantasyplayer/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Getting player {sleeperId} from redis.");
    var player = fantasyplayers.Where(x => x.SleeperId == sleeperId).FirstOrDefault();
    return player;
    
});

// Get all the players using Redis.OM
app.MapGet("/fantasyplayers", () => 
{
    Console.WriteLine($"Getting all players from redis.");
    var players = fantasyplayers.ToList();
    players = players.OrderBy(x => x.SearchRank).ToList();
    return players;
    
});

// Get my players from redis.Om
app.MapGet("/myplayers", () => 
{
    Console.WriteLine($"Getting all my players from redis.");
    var players = fantasyplayers.Where(x => x.IsOnMyTeam == true).ToArray();
    return players;
    
});

app.MapGet("/availableplayers", () => 
{
    Console.WriteLine($"Getting all available players from redis.");
    var players = fantasyplayers.Where(x => x.IsTaken == false).ToArray();
    return players;
});

// Add a player to my team using Redis.OM
app.MapPost("/fantasyplayer/claim/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Claiming player {sleeperId} from redis.");
    var player = fantasyplayers.Where(x => x.SleeperId == sleeperId).FirstOrDefault();
    player.IsOnMyTeam = true;
    player.IsTaken = false;
    fantasyplayers.Update(player);
    return player;
});

// Assign a player to a team using Redis.OM
app.MapPost("/fantasyplayer/assign/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Assigning player {sleeperId} to another team.");
    var player = fantasyplayers.Where(x => x.SleeperId == sleeperId).FirstOrDefault();
    player.IsOnMyTeam = false;
    player.IsTaken = true;
    fantasyplayers.Update(player);
    return player;
});

// reset a players status using Redis.OM
app.MapPost("/fantasyplayer/reset/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Resetting player {sleeperId}.");
    var player = fantasyplayers.Where(x => x.SleeperId == sleeperId).FirstOrDefault();
    player.IsOnMyTeam = false;
    player.IsTaken = false;
    fantasyplayers.Update(player);
    return player;
});

// set a player to thumbs up
app.MapPost("/fantasyplayer/thumbsup/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Thumbs up player {sleeperId}.");
    var player = fantasyplayers.Where(x => x.SleeperId == sleeperId).FirstOrDefault();
    player.IsThumbsUp = true;
    player.IsThumbsDown = false;
    fantasyplayers.Update(player);
    return player;
});

// Set a player to thumbs down
app.MapPost("/fantasyplayer/thumbsdown/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Thumbs down player {sleeperId}.");
    var player = fantasyplayers.Where(x => x.SleeperId == sleeperId).FirstOrDefault();
    player.IsThumbsUp = false;
    player.IsThumbsDown = true;
    fantasyplayers.Update(player);
    return player;
});

app.Run();

