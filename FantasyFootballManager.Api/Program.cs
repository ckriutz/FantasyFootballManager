using System;
using System.Linq;
using FantasyFootballManager.DataService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");

Console.WriteLine($"SQL Connection String: {sqlConnectionString}");

var builder = WebApplication.CreateBuilder(args);
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                        policy.WithOrigins("http://localhost:3000", "http://ffootball.system-k.io/", "https://ffootball.system-k.io/")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS for everything
app.UseCors(MyAllowSpecificOrigins);

using var scope = app.Services.CreateScope();
using var dbContext = scope.ServiceProvider.GetRequiredService<FantasyDbContext>();

app.MapGet("/version", () => "1.1.0");

app.MapGet("/datastatus", () => 
{ 
    var dataStatus = dbContext.DataStatus.ToArray();
    return dataStatus;
});

// Get a single player by SleeperId using Redis.OM
app.MapGet("/fantasyplayer/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Getting player {sleeperId} from the database.");
    
    var combinedQuery = from sleeper in dbContext.SleeperPlayers where sleeper.PlayerId == sleeperId
    join fantasy in dbContext.FantasyPlayers on sleeper.PlayerId equals fantasy.PlayerId
    join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FantasyDataId equals sportsdata.PlayerID
    join pros in dbContext.FantasyProsPlayers on sleeper.YahooId.ToString() equals pros.PlayerYahooId
    select new 
    {
        sleeper,
        fantasy,
        sportsdata,
        pros,
        Team = sleeper.Team
    };

    return combinedQuery.FirstOrDefault();
    
});

// Get all the players using Redis.OM
app.MapGet("/simplefantasyplayers", () => 
{
    Console.WriteLine($"Getting all players from database with selected fields.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers
    join fantasy in dbContext.FantasyPlayers on sleeper.PlayerId equals fantasy.PlayerId
    join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FantasyDataId equals sportsdata.PlayerID
    join pros in dbContext.FantasyProsPlayers on sleeper.YahooId.ToString() equals pros.PlayerYahooId
    select new 
    {
        SleeperId = sleeper.PlayerId,
        Name = sleeper.FullName,
        Position = sleeper.Position,
        Depth = sleeper.DepthChartOrder,
        ByeWeek = pros.PlayerByeWeek,
        FantasyPlayer = fantasy,
        Rank = pros.RankEcr,
        AdpPpr = sportsdata.AverageDraftPositionPPR,
        ProjPoints = sportsdata.ProjectedFantasyPoints,
        LastSeasonProjPoints = sportsdata.LastSeasonFantasyPoints,
        SearchRank = sleeper.SearchRank,
        RankEcr = pros.RankEcr,
        Team = sleeper.Team // Assuming there's a navigation property from SleeperPlayers to Teams
    };

    return combinedQuery.Where(x => x.SearchRank != 9999999).OrderBy(x => x.SearchRank).ToList();
});

app.MapGet("/fantasyplayers/search/position/{position}", (string position) => 
{
    Console.WriteLine($"Getting all players from the database with the position {position}.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers where sleeper.Position == position
    join fantasy in dbContext.FantasyPlayers on sleeper.PlayerId equals fantasy.PlayerId
    join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FantasyDataId equals sportsdata.PlayerID
    join pros in dbContext.FantasyProsPlayers on sleeper.YahooId.ToString() equals pros.PlayerYahooId
    select new 
    {
        SleeperId = sleeper.PlayerId,
        Name = sleeper.FullName,
        Position = sleeper.Position,
        Depth = sleeper.DepthChartOrder,
        ByeWeek = pros.PlayerByeWeek,
        FantasyPlayer = fantasy,
        Rank = pros.RankEcr,
        AdpPpr = sportsdata.AverageDraftPositionPPR,
        ProjPoints = sportsdata.ProjectedFantasyPoints,
        LastSeasonProjPoints = sportsdata.LastSeasonFantasyPoints,
        SearchRank = sleeper.SearchRank,
        RankEcr = pros.RankEcr,
        Team = sleeper.Team // Assuming there's a navigation property from SleeperPlayers to Teams
    };

    return combinedQuery.Where(x => x.SearchRank != 9999999).OrderBy(x => x.SearchRank).ToList();
});

app.MapGet("/rankedfantasyplayers", () => 
{
    Console.WriteLine($"Getting all ranked players from redis.");
    //var players = fantasyplayers.Where(x => x.Tier > 0).ToList();
});

// Get my players from the database.
app.MapGet("/myplayers", () => 
{
    Console.WriteLine($"Getting all my players from the database.");
    // Get all the players from FantasyPlayers where IsOnMyTeam is true
    var combinedQuery = from fantasy in dbContext.FantasyPlayers where fantasy.IsOnMyTeam == true
    join sleeper in dbContext.SleeperPlayers on fantasy.PlayerId equals sleeper.PlayerId
    join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FantasyDataId equals sportsdata.PlayerID
    join pros in dbContext.FantasyProsPlayers on sleeper.YahooId.ToString() equals pros.PlayerYahooId
    select new 
    {
        sleeper,
        fantasy,
        sportsdata,
        pros,
        Team = sleeper.Team
    };
    return combinedQuery.ToList();
    
});

app.MapGet("/availableplayers", () => 
{
    Console.WriteLine($"Getting all available players from redis.");
    //var players = fantasyplayers.Where(x => x.IsTaken == false).ToArray();
    //return players;
});

// Add a player to my team by updating the datbase.
app.MapPost("/fantasyplayer/claim/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Claiming player {sleeperId}.");
    var player = dbContext.FantasyPlayers.Where(x => x.PlayerId == sleeperId).FirstOrDefault();
    if(player == null)
    {
        return null;
    }
    player.IsOnMyTeam = true;
    player.IsTaken = false;
    dbContext.FantasyPlayers.Update(player);
    dbContext.SaveChanges();
    return player;
});

// Assign a player to a team by updating the database.
app.MapPost("/fantasyplayer/assign/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Assigning player {sleeperId} to another team.");
    var player = dbContext.FantasyPlayers.Where(x => x.PlayerId == sleeperId).FirstOrDefault();
    if(player == null)
    {
        return null;
    }
    player.IsOnMyTeam = false;
    player.IsTaken = true;
    dbContext.FantasyPlayers.Update(player);
    dbContext.SaveChanges();
    return player;
});

// reset a players status by updating the database.
app.MapPost("/fantasyplayer/reset/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Resetting player {sleeperId}.");
    var player = dbContext.FantasyPlayers.Where(x => x.PlayerId == sleeperId).FirstOrDefault();
    if(player == null)
    {
        return null;
    }
    player.IsOnMyTeam = false;
    player.IsTaken = false;
    dbContext.FantasyPlayers.Update(player);
    dbContext.SaveChanges();
    return player;
});

// set a player to thumbs up
app.MapPost("/fantasyplayer/thumbsup/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Thumbs up player {sleeperId}.");
    var player = dbContext.FantasyPlayers.Where(x => x.PlayerId == sleeperId).FirstOrDefault();
    if(player == null)
    {
        return null;
    }
    player.IsThumbsUp = true;
    player.IsThumbsDown = false;
    dbContext.FantasyPlayers.Update(player);
    dbContext.SaveChanges();
    return player;
});

// Set a player to thumbs down
app.MapPost("/fantasyplayer/thumbsdown/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Thumbs up player {sleeperId}.");
    var player = dbContext.FantasyPlayers.Where(x => x.PlayerId == sleeperId).FirstOrDefault();
    if(player == null)
    {
        return null;
    }
    player.IsThumbsUp = false;
    player.IsThumbsDown = true;
    dbContext.FantasyPlayers.Update(player);
    dbContext.SaveChanges();
    return player;
});

// Set a player to no thumbs. This happens when we dont want thumbs up or down.
app.MapPost("/fantasyplayer/nothumbs/{sleeperId}", (string sleeperId) => 
{
    Console.WriteLine($"Thumbs up player {sleeperId}.");
    var player = dbContext.FantasyPlayers.Where(x => x.PlayerId == sleeperId).FirstOrDefault();
    if(player == null)
    {
        return null;
    }
    player.IsThumbsUp = false;
    player.IsThumbsDown = false;
    dbContext.FantasyPlayers.Update(player);
    dbContext.SaveChanges();
    return player;
});

// Set the /health endpoint to return OK
app.MapGet("/health", () => "OK");

app.Run();

