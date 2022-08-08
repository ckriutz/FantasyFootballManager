using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;


// This is our API for performing CRUD operations on the Database coming from the Web App.

namespace FantasyFootballManager.Functions
{   
    public class FootballApiHttpTrigger
    {
        private readonly Models.FantasyContext _context;

        public FootballApiHttpTrigger(Models.FantasyContext context)
        {
            _context = context;
        }

        [FunctionName("FootballHealthHttpTrigger")]
        public async Task<IActionResult> CheckHealth([HttpTrigger(AuthorizationLevel.Function, "get", Route = "health/")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request for a health check.");
            var CanConnect = await _context.Database.CanConnectAsync();
            if (CanConnect == true)
            {
                return new OkObjectResult(true);
            }

            return new BadRequestObjectResult(false);
        }

        [FunctionName("GetPlayers")]
        public async Task<IActionResult> GetPlayers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "players/")] HttpRequest req, ILogger log)
        {
            // We are going to get all the players, but only the ones updated this year.
            // This was surprisingly easy.
            log.LogInformation("C# HTTP trigger function processed a request to get all the players.");

            var players = await _context.FootballPlayers.AsQueryable().Where(p => p.LastUpdatedSportsDataIO.Year == DateTime.Now.Year).ToListAsync();

            return new OkObjectResult(players);
        }

        [FunctionName("Player")]
        public async Task<IActionResult> Players([HttpTrigger(AuthorizationLevel.Function, "get", "post", "put", Route = "player/{id:int?}")] HttpRequest req, ILogger log, int? id)
        {
            // In Theroy, this is the route for individual player activities.
            // This was surprisingly easy.
            
            if (req.Method == "POST")
            {
                log.LogInformation("POST doesn't do anything yet.");
                return new OkResult();
            }

            if (req.Method == "PUT")
            {
                log.LogInformation("C# HTTP trigger function processed a request to update (PUT) a player.");
                // Lets get the data sent, and deserialize it.
                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                Models.FootballPlayer playerData = JsonConvert.DeserializeObject<Models.FootballPlayer>(requestBody);

                // Now, we update the player data.
                _context.FootballPlayers.Update(playerData);
                await _context.SaveChangesAsync();

                // Send it all Back.
                return new OkObjectResult(playerData);
            }
            
            // If we're here, it was a simple GET
            log.LogInformation("C# HTTP trigger function processed a request to get (GET) a player.");
            var player = await _context.FootballPlayers.AsQueryable().Where(p=> p.Id == id).FirstAsync();
            return new OkObjectResult(player);
            
        }

        [FunctionName("GetAvailablePlayers")]
        public async Task<IActionResult> GetAvailablePlayers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "availableplayers/")] HttpRequest req, ILogger log)
        {
            // In Theroy, we are using this route to get ALL the players.
            // This was surprisingly easy.
            log.LogInformation("C# HTTP trigger function processed a request to get all the players.");

            var players = await _context.FootballPlayers.AsQueryable().Where(p => p.IsAvailable == true).ToListAsync();

            return new OkObjectResult(players);
        }

        [FunctionName("GetAllPositions")]
        public async Task<IActionResult> GetAllPositions([HttpTrigger(AuthorizationLevel.Function, "get", Route = "positions/")] HttpRequest req, ILogger log)
        {
            // In Theroy, we are using this route to get ALL the players.
            // This was surprisingly easy.
            log.LogInformation("C# HTTP trigger function processed a request to get all the positions.");

            var positions = await _context.FootballPlayers.AsQueryable().Select(p => p.Position).Distinct().ToListAsync();

            return new OkObjectResult(positions);
        }
    }
}
