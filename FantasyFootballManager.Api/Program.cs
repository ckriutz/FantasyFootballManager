using System;
using System.Linq;
using System.Threading;
using FantasyFootballManager.DataService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;

var postgresConnectionString = Environment.GetEnvironmentVariable("postgresConnectionString");
if (string.IsNullOrWhiteSpace(postgresConnectionString))
{
    Console.WriteLine("ERROR: postgresConnectionString environment variable is not set.");
    return;
}

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FantasyDbContext>(options => options.UseNpgsql(postgresConnectionString), ServiceLifetime.Transient);
// Unified projection service for merging player sources
builder.Services.AddScoped<IUnifiedPlayerProjectionService, UnifiedPlayerProjectionService>();
builder.Services.AddScoped<IAvailablePlayersService, AvailablePlayersService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://ffootball.caseyk.dev", "http://ffootball.caseyk.dev", "http://192.168.40.13:3000")
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

app.MapGet("/version", () => "1.5.0");

// When someone goes to the root of the API, return a welcome message.
app.MapGet("/", () => "Welcome to the Fantasy Football Manager API!");

app.MapGet("/health", () =>
{
    Console.WriteLine("Health check endpoint hit.");
    return Microsoft.AspNetCore.Http.Results.Ok("API is healthy");
});

app.MapGet("/datastatus", () =>
{
    var dataStatus = dbContext.DataStatus.ToArray();
    return dataStatus;
}).RequireCors(MyAllowSpecificOrigins);

app.MapGet("/echo/{message}", (string message) =>
{
    Console.WriteLine($"Echoing message: {message}");
    return $"Echo: {message}";
});

// Get all players with comprehensive details
app.MapGet("/players", () =>
{
    Console.WriteLine("Getting all players with comprehensive details.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers
                        join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FullName equals sportsdata.Name
                        join pros in dbContext.FantasyProsPlayers on sleeper.SportRadarId equals pros.SportsdataId
                        select new
                        {
                            SleeperData = new
                            {
                                PlayerId = sleeper.PlayerId,
                                FullName = sleeper.FullName,
                                Position = sleeper.Position,
                                TeamAbbreviation = sleeper.TeamAbbreviation,
                                DepthChartOrder = sleeper.DepthChartOrder,
                                SearchRank = sleeper.SearchRank,
                                Status = sleeper.Status,
                                Age = sleeper.Age,
                                Height = sleeper.Height,
                                Weight = sleeper.Weight,
                                YearsExp = sleeper.YearsExp,
                                College = sleeper.College,
                                InjuryStatus = sleeper.InjuryStatus,
                                InjuryNotes = sleeper.InjuryNotes
                            },
                            SportsDataIo = sportsdata,
                            FantasyPros = pros,
                            Team = sleeper.Team
                        };

    return combinedQuery.Where(x => x.SleeperData.SearchRank != 9999999)
                       .OrderBy(x => x.SleeperData.SearchRank)
                       .ToList();
});

// Get all players from the database with selected fields
app.MapGet("/players/simple", () =>
{
    Console.WriteLine($"Getting all players from database with selected fields.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers
                        join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FullName equals sportsdata.Name
                        join pros in dbContext.FantasyProsPlayers on sleeper.SportRadarId equals pros.SportsdataId
                        select new
                        {
                            SleeperId = sleeper.PlayerId,
                            Name = sleeper.FullName,
                            Position = sleeper.Position,
                            Depth = sleeper.DepthChartOrder,
                            ByeWeek = pros.PlayerByeWeek,
                            Rank = pros.RankEcr,
                            AdpPpr = sportsdata.AverageDraftPositionPPR,
                            ProjPoints = sportsdata.ProjectedFantasyPoints,
                            LastSeasonProjPoints = sportsdata.LastSeasonFantasyPoints,
                            SearchRank = sleeper.SearchRank,
                            RankEcr = pros.RankEcr,
                            Team = sleeper.Team
                        };

    return combinedQuery.Where(x => x.SearchRank != 9999999).OrderBy(x => x.SearchRank).ToList();
});

app.MapGet("/players/simple/{sub}", (string sub) =>
{
    Console.WriteLine($"Getting all players from database with selected fields for user {sub}.");

    var combinedQuery = dbContext.SleeperPlayers
        .Join(dbContext.SportsDataIoPlayers, 
              sleeper => sleeper.FullName, 
              sportsdata => sportsdata.Name, 
              (sleeper, sportsdata) => new { sleeper, sportsdata })
        .Join(dbContext.FantasyProsPlayers,
              combined => combined.sleeper.SportRadarId,
              pros => pros.SportsdataId,
              (combined, pros) => new { combined.sleeper, combined.sportsdata, pros })
        .GroupJoin(dbContext.FantasyActivities.Where(a => a.User == sub),
                   combined => combined.sleeper.PlayerId,
                   activity => activity.PlayerId.ToString(),
                   (combined, activities) => new { combined, activities })
        .SelectMany(x => x.activities.DefaultIfEmpty(),
                   (combined, activity) => new
                   {
                       SleeperId = combined.combined.sleeper.PlayerId,
                       Name = combined.combined.sleeper.FullName,
                       Position = combined.combined.sleeper.Position,
                       Depth = combined.combined.sleeper.DepthChartOrder,
                       ByeWeek = combined.combined.pros.PlayerByeWeek,
                       Rank = combined.combined.pros.RankEcr,
                       AdpPpr = combined.combined.sportsdata.AverageDraftPositionPPR,
                       ProjPoints = combined.combined.sportsdata.ProjectedFantasyPoints,
                       LastSeasonProjPoints = combined.combined.sportsdata.LastSeasonFantasyPoints,
                       SearchRank = combined.combined.sleeper.SearchRank,
                       RankEcr = combined.combined.pros.RankEcr,
                       Team = combined.combined.sleeper.Team,
                       // FantasyActivity fields with default values
                       IsThumbsUp = activity != null && activity.IsThumbsUp,
                       IsThumbsDown = activity != null && activity.IsThumbsDown,
                       IsDraftedOnMyTeam = activity != null && activity.IsDraftedOnMyTeam,
                       IsDraftedOnOtherTeam = activity != null && activity.IsDraftedOnOtherTeam,
                       ActivityUser = activity != null ? activity.User : null
                   })
        .Where(x => x.SearchRank != 9999999)
        .OrderBy(x => x.SearchRank);

    return combinedQuery.ToList();
});

// Get a single player by SleeperId
app.MapGet("/players/{sleeperId}", (string sleeperId) =>
{
    Console.WriteLine($"Getting player {sleeperId} from the database.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers
                        where sleeper.PlayerId == sleeperId
                        join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FullName equals sportsdata.Name
                        join pros in dbContext.FantasyProsPlayers on sleeper.SportRadarId equals pros.SportsdataId
                        select new
                        {
                            SleeperData = new
                            {
                                PlayerId = sleeper.PlayerId,
                                FullName = sleeper.FullName,
                                Position = sleeper.Position,
                                TeamAbbreviation = sleeper.TeamAbbreviation,
                                DepthChartOrder = sleeper.DepthChartOrder,
                                SearchRank = sleeper.SearchRank,
                                Status = sleeper.Status,
                                Age = sleeper.Age,
                                Height = sleeper.Height,
                                Weight = sleeper.Weight,
                                YearsExp = sleeper.YearsExp,
                                College = sleeper.College
                            },
                            SportsDataIo = sportsdata,
                            FantasyPros = pros,
                            Team = sleeper.Team
                        };

    return combinedQuery.FirstOrDefault();

});

app.MapGet("/players/{sleeperId}/activity/{sub}", (string sleeperId, string sub) =>
{
    Console.WriteLine($"Getting player {sleeperId} activity for user {sub}.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers
                        where sleeper.PlayerId == sleeperId
                        join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FullName equals sportsdata.Name
                        join pros in dbContext.FantasyProsPlayers on sleeper.SportRadarId equals pros.SportsdataId
                        join activity in dbContext.FantasyActivities.Where(a => a.User == sub) on sleeper.PlayerId equals activity.PlayerId.ToString() into activityGroup
                        from activity in activityGroup.DefaultIfEmpty()
                        select new
                        {
                            SleeperData = new
                            {
                                PlayerId = sleeper.PlayerId,
                                FullName = sleeper.FullName,
                                Position = sleeper.Position,
                                TeamAbbreviation = sleeper.TeamAbbreviation,
                                DepthChartOrder = sleeper.DepthChartOrder,
                                SearchRank = sleeper.SearchRank,
                                Status = sleeper.Status,
                                Age = sleeper.Age,
                                Height = sleeper.Height,
                                Weight = sleeper.Weight,
                                YearsExp = sleeper.YearsExp,
                                College = sleeper.College
                            },
                            SportsDataIo = sportsdata,
                            FantasyPros = pros,
                            Team = sleeper.Team,
                            // FantasyActivity fields with default values
                            IsThumbsUp = activity != null && activity.IsThumbsUp,
                            IsThumbsDown = activity != null && activity.IsThumbsDown,
                            IsDraftedOnMyTeam = activity != null && activity.IsDraftedOnMyTeam,
                            IsDraftedOnOtherTeam = activity != null && activity.IsDraftedOnOtherTeam,
                            ActivityUser = activity != null ? activity.User : null
                        };

    return combinedQuery.FirstOrDefault();

});

// Get players by position
app.MapGet("/players/position/{position}", (string position) => 
{
    Console.WriteLine($"Getting all players with position {position}.");

    var combinedQuery = from sleeper in dbContext.SleeperPlayers where sleeper.Position == position
    join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FullName equals sportsdata.Name
    join pros in dbContext.FantasyProsPlayers on sleeper.SportRadarId equals pros.SportsdataId
    select new 
    {
        SleeperId = sleeper.PlayerId,
        Name = sleeper.FullName,
        Position = sleeper.Position,
        Depth = sleeper.DepthChartOrder,
        ByeWeek = pros.PlayerByeWeek,
        Rank = pros.RankEcr,
        AdpPpr = sportsdata.AverageDraftPositionPPR,
        ProjPoints = sportsdata.ProjectedFantasyPoints,
        LastSeasonProjPoints = sportsdata.LastSeasonFantasyPoints,
        SearchRank = sleeper.SearchRank,
        RankEcr = pros.RankEcr,
        Team = sleeper.Team
    };

    return combinedQuery.Where(x => x.SearchRank != 9999999).OrderBy(x => x.SearchRank).ToList();
});

// Get players drafted on my team
app.MapGet("/players/drafted/{sub}", (string sub) => 
{
    var combinedQuery = dbContext.SleeperPlayers
        .Join(dbContext.SportsDataIoPlayers, 
              sleeper => sleeper.FullName, 
              sportsdata => sportsdata.Name, 
              (sleeper, sportsdata) => new { sleeper, sportsdata })
        .Join(dbContext.FantasyProsPlayers,
              combined => combined.sleeper.SportRadarId,
              pros => pros.SportsdataId,
              (combined, pros) => new { combined.sleeper, combined.sportsdata, pros })
        .GroupJoin(dbContext.FantasyActivities.Where(a => a.User == sub && a.IsDraftedOnMyTeam),
                   combined => combined.sleeper.PlayerId,
                   activity => activity.PlayerId.ToString(),
                   (combined, activities) => new { combined, activities })
        .SelectMany(x => x.activities.DefaultIfEmpty(),
                   (combined, activity) => new
                   {
                       SleeperId = combined.combined.sleeper.PlayerId,
                       Name = combined.combined.sleeper.FullName,
                       Position = combined.combined.sleeper.Position,
                       Depth = combined.combined.sleeper.DepthChartOrder,
                       ByeWeek = combined.combined.pros.PlayerByeWeek,
                       Rank = combined.combined.pros.RankEcr,
                       AdpPpr = combined.combined.sportsdata.AverageDraftPositionPPR,
                       ProjPoints = combined.combined.sportsdata.ProjectedFantasyPoints,
                       LastSeasonProjPoints = combined.combined.sportsdata.LastSeasonFantasyPoints,
                       SearchRank = combined.combined.sleeper.SearchRank,
                       RankEcr = combined.combined.pros.RankEcr,
                       Team = combined.combined.sleeper.Team,
                       // FantasyActivity fields with default values
                       IsThumbsUp = activity != null && activity.IsThumbsUp,
                       IsThumbsDown = activity != null && activity.IsThumbsDown,
                       IsDraftedOnMyTeam = activity != null && activity.IsDraftedOnMyTeam,
                       IsDraftedOnOtherTeam = activity != null && activity.IsDraftedOnOtherTeam,
                       ActivityUser = activity != null ? activity.User : null
                   })
        .Where(x => x.IsDraftedOnMyTeam)
        .OrderBy(x => x.SearchRank);

    return combinedQuery.ToList();
});

// Get top available players for a user (excluding their drafted roster) with optional tuning parameters
app.MapGet("/players/available/{sub}", async (
    string sub,
    int? overallLimit,
    int? perPositionLimit,
    bool? includeK,
    bool? includeDst,
    bool? biasToNeeds,
    int? needsMultiplier,
    int? hardCap,
    IAvailablePlayersService availableService,
    CancellationToken ct) =>
{
    Console.WriteLine($"Getting available players for user {sub}.");
    var options = new AvailablePlayersQueryOptions(
        OverallLimit: overallLimit ?? 40,
        PerPositionLimit: perPositionLimit ?? 12,
        IncludeK: includeK ?? false,
        IncludeDst: includeDst ?? false,
        BiasToNeeds: biasToNeeds ?? true,
        NeedsMultiplier: needsMultiplier ?? 4,
        HardCap: hardCap ?? 60
    ).Normalize();

    var list = await availableService.GetTopAvailableAsync(sub, options, ct);
    return list;
});

// Add a player to my team by updating the datbase.
app.MapPost("/players/{sleeperId}/draft/{sub}", (string sleeperId, string sub) =>
{
    Console.WriteLine($"Drafting player {sleeperId}.");
    var player = dbContext.FantasyActivities.Where(x => x.PlayerId.ToString() == sleeperId && x.User == sub).FirstOrDefault();
    if (player == null)
    {
        player = new FantasyActivity
        {
            PlayerId = int.Parse(sleeperId),
            User = sub,
            IsThumbsUp = false,
            IsThumbsDown = false,
            IsDraftedOnMyTeam = true,
            IsDraftedOnOtherTeam = false
        };
        dbContext.FantasyActivities.Add(player);
    }
    else
    {
        player.IsDraftedOnMyTeam = true;
        player.IsDraftedOnOtherTeam = false;
        dbContext.FantasyActivities.Update(player);
    }

    
    dbContext.SaveChanges();
    return player;
});

// Assign a player to a team by updating the database.
app.MapPost("/players/{sleeperId}/assign/{sub}", (string sleeperId, string sub) =>
{
    Console.WriteLine($"Assigning player {sleeperId}.");
    var player = dbContext.FantasyActivities.Where(x => x.PlayerId.ToString() == sleeperId && x.User == sub).FirstOrDefault();
    if (player == null)
    {
        player = new FantasyActivity
        {
            PlayerId = int.Parse(sleeperId),
            User = sub,
            IsThumbsUp = false,
            IsThumbsDown = false,
            IsDraftedOnMyTeam = false,
            IsDraftedOnOtherTeam = true
        };
        dbContext.FantasyActivities.Add(player);
    }
    else
    {
        player.IsDraftedOnMyTeam = false;
        player.IsDraftedOnOtherTeam = true;
        dbContext.FantasyActivities.Update(player);
    }

    
    dbContext.SaveChanges();
    return player;
});

// reset a players status by updating the database.
app.MapPost("/players/{sleeperId}/reset/{sub}", (string sleeperId, string sub) =>
{
    Console.WriteLine($"Resetting player {sleeperId}.");
    var player = dbContext.FantasyActivities.Where(x => x.PlayerId.ToString() == sleeperId && x.User == sub).FirstOrDefault();
    if (player == null)
    {
        player = new FantasyActivity
        {
            PlayerId = int.Parse(sleeperId),
            User = sub,
            IsThumbsUp = false,
            IsThumbsDown = false,
            IsDraftedOnMyTeam = false,
            IsDraftedOnOtherTeam = false
        };
        dbContext.FantasyActivities.Add(player);
    }
    else
    {
        player.IsDraftedOnMyTeam = false;
        player.IsDraftedOnOtherTeam = false;
        dbContext.FantasyActivities.Update(player);
    }

    
    dbContext.SaveChanges();
    return player;
});

// Set a player to thumbs up
app.MapPost("/players/{sleeperId}/thumbsup/{sub}", (string sleeperId, string sub) => 
{
    Console.WriteLine($"Thumbs up player {sleeperId} for user {sub}.");
    var player = dbContext.FantasyActivities.FirstOrDefault(x => x.PlayerId.ToString() == sleeperId && x.User == sub);
    if (player == null)
    {
        player = new FantasyActivity
        {
            PlayerId = int.Parse(sleeperId),
            User = sub,
            IsThumbsUp = true,
            IsThumbsDown = false
        };
        dbContext.FantasyActivities.Add(player);
    }
    else
    {
        player.IsThumbsUp = !player.IsThumbsUp;
        player.IsThumbsDown = false;
        dbContext.FantasyActivities.Update(player);
    }
    dbContext.SaveChanges();
    return player;
});

// Set a player to thumbs down
app.MapPost("/players/{sleeperId}/thumbsdown/{sub}", (string sleeperId, string sub) => 
{
    Console.WriteLine($"Thumbs down player {sleeperId} for user {sub}.");
    var player = dbContext.FantasyActivities.FirstOrDefault(x => x.PlayerId.ToString() == sleeperId && x.User == sub);
    if (player == null)
    {
        player = new FantasyActivity
        {
            PlayerId = int.Parse(sleeperId),
            User = sub,
            IsThumbsUp = false,
            IsThumbsDown = true
        };
        dbContext.FantasyActivities.Add(player);
    }
    else
    {
        player.IsThumbsUp = false;
        player.IsThumbsDown = !player.IsThumbsDown;
        dbContext.FantasyActivities.Update(player);
    }
    dbContext.SaveChanges();
    return player;
});

// Add endpoint to get a user by Auth0Id
app.MapGet("/users/{auth0Id}", (string auth0Id) =>
{
    Console.WriteLine($"Fetching user with Auth0Id: {auth0Id}");
    var user = dbContext.Users.FirstOrDefault(u => u.Auth0Id == auth0Id);
    return user ?? null;
});

// Add endpoint to create or update a user by Auth0Id
app.MapPost("/users", (User user) =>
{
    Console.WriteLine($"Creating or updating user with Auth0Id: {user.Auth0Id}");
    var existingUser = dbContext.Users.FirstOrDefault(u => u.Auth0Id == user.Auth0Id);

    if (existingUser != null)
    {
        // Update existing user
        existingUser.YahooUsername = user.YahooUsername;
        existingUser.YahooLeagueId = user.YahooLeagueId;
        existingUser.EspnUsername = user.EspnUsername;
        existingUser.EspnLeagueId = user.EspnLeagueId;
        existingUser.SleeperUsername = user.SleeperUsername;
        existingUser.SleeperLeagueId = user.SleeperLeagueId;
    }
    else
    {
        // Add new user
        dbContext.Users.Add(user);
    }

    dbContext.SaveChanges();
    return user;
});

app.Run();