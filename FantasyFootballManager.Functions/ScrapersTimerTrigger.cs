using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FantasyFootballManager.Functions
{
    public class ScrapersTimerTrigger
    {
        private readonly string _connectionString;
        private static readonly string _footballCalculatorQueueName = "footballcalculator";
        private static readonly string _sportsDataIoQueueName = "sportsdataio";
        private static readonly string _fantasyProsQueueName = "fantasypros";
        private static readonly HttpClient _client = new HttpClient();

        public ScrapersTimerTrigger()
        {
            _connectionString = System.Environment.GetEnvironmentVariable("QueueStorageConnectionString");
            
        }

        // Every 5 Minutes: 0 */5 * * * *
        // Every 12 Hours: 0 0 */12 * * *
        [FunctionName("ScrapersTimerTrigger")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await RunSportsDataIoScraper(log);
            await RunFantasyFootballCalculatorScraper(log);
            await RunFantasyProsScraper(log);
        }

        private async Task RunSportsDataIoScraper(ILogger log)
        {
            Console.WriteLine("Time to hit the SportsData.IO API for data!");
            var _queueClient = new QueueClient(_connectionString, _sportsDataIoQueueName);
            var OcpApimSubscriptionKey = System.Environment.GetEnvironmentVariable("OcpApimSubscriptionKey");
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", OcpApimSubscriptionKey);

            var response = await _client.GetStringAsync("https://fly.sportsdata.io/v3/nfl/stats/json/FantasyPlayers");

            List<Models.SportsDataIo.SportsDataIOObject> playerData = JsonSerializer.Deserialize<List<Models.SportsDataIo.SportsDataIOObject>>(response);
            
            log.LogInformation($"SportsData.io API returned {playerData.Count} players.");

            // This is to fix some null things.
            foreach (Models.SportsDataIo.SportsDataIOObject p in playerData)
            {
                if (p.ByeWeek == null)
                {
                    p.ByeWeek = 0;
                }
                if (p.AverageDraftPositionIDP == null)
                {
                    p.AverageDraftPositionIDP = 0;
                }
            }

            // Now lets chuck it into a Storage Queue.
            playerData.ForEach(p => _queueClient.SendMessage(Base64Encode(JsonSerializer.Serialize(p))));
        }

        private async Task RunFantasyFootballCalculatorScraper(ILogger log)
        {
            log.LogInformation("Time to hit the Fantasy Football Calculator API!");
            var _queueClient = new QueueClient(_connectionString, _footballCalculatorQueueName);
            var response = await _client.GetStringAsync("https://fantasyfootballcalculator.com/api/v1/adp/standard?teams=12&year=2021&position=all");

            Models.FantasyFootballCalculator.Root responseRoot = JsonSerializer.Deserialize<Models.FantasyFootballCalculator.Root>(response);

            log.LogInformation($"Fantasy Football Calculator API returned {responseRoot.players.Count} players.");
            
            // Now lets chuck it into a Storage Queue.
            responseRoot.players.ForEach(p => _queueClient.SendMessage(Base64Encode(JsonSerializer.Serialize(p))));
        }

        private async Task RunFantasyProsScraper(ILogger log)
        {
            log.LogInformation("Time to scrape FantasyPros for Data.");
            var _queueClient = new QueueClient(_connectionString, _fantasyProsQueueName);
            string responseBody = await _client.GetStringAsync("https://partners.fantasypros.com/api/v1/consensus-rankings.php?sport=NFL&year=2021&week=0&id=1054&position=ALL&type=ST&scoring=HALF&filters=7:9:285:699:747&export=json");

            Models.FantasyPros.Root players = JsonSerializer.Deserialize<Models.FantasyPros.Root>(responseBody);

            log.LogInformation($"Fantasy Pros API returned {players.players.Count} players.");
            
            // Now lets chuck it into a Storage Queue.
            players.players.ForEach(p => _queueClient.SendMessage(Base64Encode(JsonSerializer.Serialize(p))));
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
