using System;
using System.Linq;
using System.Text.Json;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FantasyFootballManager.Functions
{
    public class ScrapersQueueTrigger
    {
        private readonly Models.FantasyContext _context;

        public ScrapersQueueTrigger(Models.FantasyContext context)
        {
            _context = context;
        }

        [FunctionName("SportsDataIoQueueTrigger")]
        public void RunSportsDataIoQueueTrigger([QueueTrigger("sportsdataio", Connection = "QueueStorageConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"SportsData.io Queue trigger function processed: {myQueueItem}");

            // First step, lets turn this into a SportsDataIo Object.
            var sportsDataIoPlayer = JsonSerializer.Deserialize<Models.SportsDataIo.SportsDataIOObject>(myQueueItem);

            // Second Step, lets turn this into an object with data we care about.
            Models.FootballPlayer player = new Models.FootballPlayer
            {
                SportsDataIOId = sportsDataIoPlayer.PlayerID,
                Name = sportsDataIoPlayer.Name,
                Position = sportsDataIoPlayer.Position,
                Team = sportsDataIoPlayer.Team,
                Bye = sportsDataIoPlayer.ByeWeek.HasValue ? sportsDataIoPlayer.ByeWeek.Value : 0,
                AverageDraftPositionSportsData = sportsDataIoPlayer.AverageDraftPositionPPR,
                LastSeasonFantasyPoints = sportsDataIoPlayer.LastSeasonFantasyPoints,
                ProjectedFantasyPoints = sportsDataIoPlayer.ProjectedFantasyPoints,
                AuctionValue = sportsDataIoPlayer.AuctionValue,
                LastUpdatedSportsDataIO = DateTime.UtcNow,
                IsAvailable = true
            };

            var existingPlayer = _context.FootballPlayers.FirstOrDefault(f => f.SportsDataIOId == player.SportsDataIOId);
            if(existingPlayer == null)
            {
                log.LogInformation("Adding a new player to the database!");
                // This person isn't in the database, so lets add them.
                _context.FootballPlayers.Add(player);
                _context.SaveChanges();
            }
            else
            {
                log.LogInformation("Updating an existing player!");
                existingPlayer.Position = player.Position;
                existingPlayer.Team = player.Team;
                existingPlayer.Bye = player.Bye;
                existingPlayer.AverageDraftPositionSportsData = player.AverageDraftPositionSportsData;
                existingPlayer.LastSeasonFantasyPoints = player.LastSeasonFantasyPoints;
                existingPlayer.ProjectedFantasyPoints = player.ProjectedFantasyPoints;
                existingPlayer.AuctionValue = player.AuctionValue;
                existingPlayer.LastUpdatedSportsDataIO = player.LastUpdatedSportsDataIO;
                _context.Update(existingPlayer);
                _context.SaveChanges();
            }
        }
    
        [FunctionName("FantasyProsQueueTrigger")]
        public void RunFantasyProsQueueTrigger([QueueTrigger("fantasypros", Connection = "QueueStorageConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"FantasyPros Queue trigger function processed: {myQueueItem}");

            // First step, lets turn this into a FProsDataObject.
            var FProsPlayer = JsonSerializer.Deserialize<Models.FantasyPros.Player>(myQueueItem);

            // Second Step, lets turn this into an object with data we care about.
            Models.FootballPlayer player = new Models.FootballPlayer
            {
                FantasyProsId = FProsPlayer.player_id,
                Name = FProsPlayer.player_name,
                PlayerHeadshotURL = FProsPlayer.player_square_image_url,
                FantasyProsRank = FProsPlayer.rank_ecr,
                Tier = FProsPlayer.tier,
                LastUpdatedFantasyPros = DateTime.UtcNow
            };

            // Thrid Step, lets write/update this to the database.

            // Is there an entry already in the database? (There should be)
            var existingPlayerById = _context.FootballPlayers.FirstOrDefault(f => f.FantasyProsId == player.FantasyProsId);
            if(existingPlayerById == null)
            {
                // Okay, lets quickly check to see if we can find them by name...
                var existingPlayerByName = _context.FootballPlayers.FirstOrDefault(f => f.Name == player.Name);
                if (existingPlayerByName != null)
                {
                    // Okay, found them the hard way!
                    log.LogInformation("Updating an existing player by Name with Fantasy Pros data.");
                    existingPlayerByName.FantasyProsId = player.FantasyProsId;
                    existingPlayerByName.FantasyProsRank = player.FantasyProsRank;
                    existingPlayerByName.Tier = player.Tier;
                    existingPlayerByName.PlayerHeadshotURL = player.PlayerHeadshotURL;
                    existingPlayerByName.LastUpdatedFantasyPros = player.LastUpdatedFantasyPros;
                    _context.Update(existingPlayerByName);
                    _context.SaveChanges();
                }
                else
                {
                    // Wow, okay. They don't exsit at all?
                    log.LogInformation("I'm going to skip adding a new Player using FantasyPros data!");
                }
            }
            else
            {
                // Okay, found them the easy way!
                log.LogInformation("Updating an existing player by Id with Fantasy Pros data.");
                existingPlayerById.FantasyProsRank = player.FantasyProsRank;
                existingPlayerById.Tier = player.Tier;
                existingPlayerById.PlayerHeadshotURL = player.PlayerHeadshotURL;
                existingPlayerById.LastUpdatedFantasyPros = player.LastUpdatedFantasyPros;
                _context.Update(existingPlayerById);
                _context.SaveChanges();
            }

            
        }
    
        [FunctionName("FantasyFootballCalculatorTrigger")]
        public void RunFantasyFootballCalculatorTrigger([QueueTrigger("footballcalculator", Connection = "QueueStorageConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"Fantasy Football Calculator Queue trigger function processed: {myQueueItem}");
        
            // First step, lets turn this into a FootballCalculatorDataObject.
            var FCalcPlayer = JsonSerializer.Deserialize<Models.FantasyFootballCalculator.FootballCalculatorDataObject>(myQueueItem);

            // Second Step, lets turn this into an object with data we care about.
            Models.FootballPlayer player = new Models.FootballPlayer
            {
                Name = FCalcPlayer.name,
                FootballCalculatorId = FCalcPlayer.player_id,
                FFCRank = ((FCalcPlayer.high+FCalcPlayer.low)/2),
                AverageDraftPositionFCalculator = FCalcPlayer.adp,
                LastUpdatedFFootballCalculator = DateTime.UtcNow
            };

            // Thrid Step, lets write/update this to the database.
            // Is there an entry already in the database? (There should be)
            var existingPlayerById = _context.FootballPlayers.FirstOrDefault(f => f.FootballCalculatorId == player.FootballCalculatorId);
            if(existingPlayerById == null)
            {
                // Okay, lets quickly check to see if we can find them by name...
                var existingPlayerByName = _context.FootballPlayers.FirstOrDefault(f => f.Name == player.Name);
                if (existingPlayerByName != null)
                {
                    // Okay, found them the hard way!
                    log.LogInformation("Updating an existing player by Name with Football Calculator data.");
                    existingPlayerByName.FootballCalculatorId = player.FootballCalculatorId;
                    existingPlayerByName.FFCRank = player.FFCRank;
                    existingPlayerByName.AverageDraftPositionFCalculator = player.AverageDraftPositionFCalculator;
                    existingPlayerByName.LastUpdatedFFootballCalculator = player.LastUpdatedFFootballCalculator;
                    _context.Update(existingPlayerByName);
                    _context.SaveChanges();
                }
                else
                {
                    // Wow, okay. They don't exsit at all?
                    log.LogInformation("Going to skip adding a new player from Fantasy Football Calculator");
                }
                
            }
            else
            {
                // Okay, found them the easy way!
                log.LogInformation("Updating an existing player by Id with Fantasy Football Calculator data.");
                existingPlayerById.FFCRank = player.FFCRank;
                existingPlayerById.AverageDraftPositionFCalculator = player.AverageDraftPositionFCalculator;
                existingPlayerById.LastUpdatedFFootballCalculator = player.LastUpdatedFFootballCalculator;
                _context.Update(existingPlayerById);
                _context.SaveChanges();
            }
        }
    
        [FunctionName("StatusQueueTrigger")]
        public void ImportStatusesTrigger([QueueTrigger("status", Connection = "QueueStorageConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"Status Queue trigger function processed: {myQueueItem}");

            var importStatus = JsonSerializer.Deserialize<Models.ImportStatus>(myQueueItem);
            // First, lets see if we have one. We really should.
            var existingStatus = _context.ImportStatuses.FirstOrDefault(i => i.Service ==  importStatus.Service);
            if(existingStatus == null)
            {
                _context.ImportStatuses.Add(importStatus);
                _context.SaveChanges();
            }
            else
            {
                // We are in here! Lets update.
                existingStatus.LastUpdated = importStatus.LastUpdated;
                _context.Update(existingStatus);
                _context.SaveChanges();
            }
        }
    }
}
