using System;
using System.Linq;
using System.Threading;
using FantasyFootballManager.DataService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using FantasyFootballManager.Api.Services;
using System.Text.Json;

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
builder.Services.AddDbContext<FantasyDbContext>(options => options.UseNpgsql(postgresConnectionString));
// Unified projection service for merging player sources
builder.Services.AddScoped<IUnifiedPlayerService, UnifiedPlayerService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IAiInferenceService, AiInferenceService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "https://ffootball.caseyk.dev", "http://ffootball.caseyk.dev", "http://192.168.40.13:3000", "http://192.168.40.13:3001")
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

// Enable CORS for everything
app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/version", () => "1.75.0");

// When someone goes to the root of the API, return a welcome message.
app.MapGet("/", () => "Welcome to the Fantasy Football Manager API!");

app.MapGet("/health", () =>
{
    Console.WriteLine("Health check endpoint hit.");
    return Microsoft.AspNetCore.Http.Results.Ok("API is healthy");
});

app.MapGet("/datastatus", async (FantasyDbContext dbContext, CancellationToken ct) =>
{
    var dataStatus = await dbContext.DataStatus.ToArrayAsync(ct);
    return dataStatus;
}).RequireCors(MyAllowSpecificOrigins);

app.MapGet("/echo/{message}", (string message) =>
{
    Console.WriteLine($"Echoing message: {message}");
    return $"Echo: {message}";
});

// Get all players with all the fields that are remotely needed.
// At this time, we eventually need to move it to the service.
app.MapGet("/players", (FantasyDbContext dbContext) =>
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

// Get all the players, with a focus on the user.
// This means the players who have been assigned, drafted, or otherwise associated with the user.
app.MapGet("/players/unified/{sub}", (string sub, IPlayersService playersService) =>
{
    Console.WriteLine($"Getting all players with selected fields for user {sub}.");
    // Use the players service
    return playersService.GetUnifiedPlayersAsync(userId: sub);
});

// TODO: Create an endpoint that will show all the players, but without user specific ones.


app.MapGet("/players/{sleeperId}/activity/{sub}", (string sleeperId, string sub, IPlayersService playersService, CancellationToken ct) =>
{
    Console.WriteLine($"Getting player {sleeperId} activity for user {sub}.");
    // Use the players service to get the unified player with activity
    return playersService.GetUnifiedPlayerAsync(sleeperId, sub, ct);
});

// Get players by position
// app.MapGet("/players/position/{position}", (string position, FantasyDbContext dbContext) => 
// {
//     Console.WriteLine($"Getting all players with position {position}.");

//     var combinedQuery = from sleeper in dbContext.SleeperPlayers where sleeper.Position == position
//     join sportsdata in dbContext.SportsDataIoPlayers on sleeper.FullName equals sportsdata.Name
//     join pros in dbContext.FantasyProsPlayers on sleeper.SportRadarId equals pros.SportsdataId
//     select new 
//     {
//         SleeperId = sleeper.PlayerId,
//         Name = sleeper.FullName,
//         Position = sleeper.Position,
//         Depth = sleeper.DepthChartOrder,
//         ByeWeek = pros.PlayerByeWeek,
//         Rank = pros.RankEcr,
//         AdpPpr = sportsdata.AverageDraftPositionPPR,
//         ProjPoints = sportsdata.ProjectedFantasyPoints,
//         LastSeasonProjPoints = sportsdata.LastSeasonFantasyPoints,
//         SearchRank = sleeper.SearchRank,
//         RankEcr = pros.RankEcr,
//         Team = sleeper.Team
//     };

//     return combinedQuery.Where(x => x.SearchRank != 9999999).OrderBy(x => x.SearchRank).ToList();
// });

app.MapGet("/players/drafted/{sub}", (string sub, IPlayersService playersService, CancellationToken ct) =>
{
    Console.WriteLine($"Getting drafted players for user {sub}.");
    return playersService.GetRosterAsync(sub, ct);
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
    IPlayersService availableService,
    CancellationToken ct) =>
{
    Console.WriteLine($"Getting available players for user {sub}.");
    var options = new PlayersQueryOptions(
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
app.MapPost("/players/{sleeperId}/draft/{sub}", (string sleeperId, string sub, FantasyDbContext dbContext) =>
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
app.MapPost("/players/{sleeperId}/assign/{sub}", (string sleeperId, string sub, FantasyDbContext dbContext) =>
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
app.MapPost("/players/{sleeperId}/reset/{sub}", (string sleeperId, string sub, FantasyDbContext dbContext) =>
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
app.MapPost("/players/{sleeperId}/thumbsup/{sub}", (string sleeperId, string sub, FantasyDbContext dbContext) => 
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
app.MapPost("/players/{sleeperId}/thumbsdown/{sub}", (string sleeperId, string sub, FantasyDbContext dbContext) => 
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
app.MapGet("/users/{auth0Id}", (string auth0Id, FantasyDbContext dbContext) =>
{
    Console.WriteLine($"Fetching user with Auth0Id: {auth0Id}");
    var user = dbContext.Users.FirstOrDefault(u => u.Auth0Id == auth0Id);
    return user ?? null;
});

// Add endpoint to create or update a user by Auth0Id
app.MapPost("/users/{sub}", (User user, FantasyDbContext dbContext) =>
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

app.MapGet("/ai/draft-reccomendations/{sub}", async (string sub, IPlayersService playersService, IAiInferenceService aiInferenceService, CancellationToken ct) =>
{
    Console.WriteLine($"Generating draft recommendations for user {sub}.");
    // First step, we need to get the players the user has already drafted.
    var draftedPlayers = await playersService.GetRosterAsync(sub, ct);

    // Then, get the top players available
    var options = new PlayersQueryOptions(
        OverallLimit: 40,
        PerPositionLimit: 12,
        IncludeK: true,
        IncludeDst: false,
        BiasToNeeds: true,
        NeedsMultiplier: 4,
        HardCap: 60
    ).Normalize();

    var topPlayers = await playersService.GetTopAvailableAsync(sub, options, ct);

    // Convert to lightweight DTOs for AI processing
    var aiDraftedPlayers = draftedPlayers.Select(AiUnifiedPlayer.FromUnifiedPlayer).ToList();
    var aiAvailablePlayers = topPlayers.Select(AiUnifiedPlayer.FromUnifiedPlayer).ToList();

    string instructions =
    """
    You are a fantasy football expert powered by AI, leveraging real-time data, advanced metrics (e.g., Expected Fantasy Points, target share, snap counts), and user-specific preferences to provide optimal draft recommendations. Your job is to analyze my current roster, available players, opponent draft tendencies, and league settings (scoring format: [PPR/half-PPR/standard], number of teams: [X], roster requirements: [e.g., 1 QB, 2 RB, 2 WR, 1 TE, 1 FLEX, 1 K, 6 bench]) to recommend 3 players to draft, prioritizing strategic fit and positional scarcity.

    **Draft Strategy**:
    - Prioritize players with IsThumbsUp set to true and avoid those with IsThumbsDown set to true unless there’s a compelling reason (e.g., significant value drop or matchup advantage).
    - Target RB and WR early unless an elite QB or TE (e.g., top-tier like Patrick Mahomes or Travis Kelce) is available at a value, considering positional scarcity and tier-based drafting.
    - Ensure the starting roster includes: 1 QB, 2 RB, 2 WR, 1 TE, 1 FLEX (RB, WR, or TE), 1 K.
    - After securing starters, recommend 6 bench players to cover bye weeks and injuries, prioritizing a balanced mix of positions (e.g., 2 RB, 2 WR, 1 QB, 1 TE) with defined roles (e.g., handcuff RBs, high-target WRs).
    - Avoid overloading the roster with players on the same bye week and flag potential conflicts (e.g., more than two players with the same bye).
    - Consider stacking opportunities (e.g., QB-WR pairs like Jalen Hurts and A.J. Brown) that align with my draft history or stated preferences.
    - Analyze opponent draft behavior (e.g., positional runs, sleeper picks) to recommend counter-strategies, such as securing a scarce position before it’s depleted.
    - Use real-time data (e.g., injury updates, depth chart changes) and advanced metrics to inform recommendations, prioritizing players with high upside, favorable matchups, or emerging roles.

    **Output Format**:
    Return your response in JSON format with the following structure for each recommendation:
    {
        "playerId": "<playerId>",
        "playerName": "<full name of the player>",
        "reason": "<1-2 sentence explanation of why this player is recommended, including projected points, matchup strength, or strategic fit>",
        "matchupStrength": "<brief note on matchup favorability, e.g., 'Favorable vs. weak pass defense'>",
        "riskLevel": "<Low/Medium/High, based on injury risk, role uncertainty, or volatility>"
    }
    DO NOT include any markdown formatting in your response.
    """;

    // Lastly we need to send the deserialized text of both the drafted players and the top available players list, along with the prompt.
    var prompt = $"{instructions}Given the following drafted players: {JsonSerializer.Serialize(aiDraftedPlayers)} " +
                 $"and the following available players: {JsonSerializer.Serialize(aiAvailablePlayers)}, " +
                 $"what are your recommendations for the draft?";

    var processedResult = await aiInferenceService.GetResponseAsync(prompt);
    return processedResult;
    //if (processedResult.Success)
    //{
        //return Microsoft.AspNetCore.Http.Results.Ok(processedResult.Recommendations);
    //}
    //else
    //{
        //return Microsoft.AspNetCore.Http.Results.BadRequest(new { 
        //    error = processedResult.ErrorMessage, 
        //    rawResponse = processedResult.RawResponse 
        //});
    //}

});

app.Run();