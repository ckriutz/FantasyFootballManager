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
        }

        public async Task RunSportsDataIoScraper(ILogger log)
        {
            Console.WriteLine("Lets do an initial load of SportsData.IO data!");
            var _queueClient = new QueueClient(_connectionString, _sportsDataIoQueueName);
            var OcpApimSubscriptionKey = System.Environment.GetEnvironmentVariable("OcpApimSubscriptionKey");
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions();
            options.IgnoreNullValues = true;

            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", OcpApimSubscriptionKey);

            var response = await _client.GetStringAsync("https://fly.sportsdata.io/v3/nfl/stats/json/FantasyPlayers");

            List<Models.SportsDataIo.SportsDataIOObject> playerData = JsonSerializer.Deserialize<List<Models.SportsDataIo.SportsDataIOObject>>(response);
            
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

            foreach (var p in playerData)
            {
                _queueClient.SendMessage(Base64Encode(JsonSerializer.Serialize(p)));
                //Console.WriteLine(p.Name);
            }
        }

        public async Task RunFantasyFootballCalculatorScraper(ILogger log)
        {
            log.LogInformation("Time to hit the Fantasy Football Calculator API!");
            var _queueClient = new QueueClient(_connectionString, _footballCalculatorQueueName);
            var response = await _client.GetStringAsync("https://fantasyfootballcalculator.com/api/v1/adp/standard?teams=12&year=2021&position=all");

            Models.FantasyFootballCalculator.Root responseRoot = JsonSerializer.Deserialize<Models.FantasyFootballCalculator.Root>(response);

            log.LogInformation($"Fantasy Football Calculator API returned {responseRoot.players.Count} players.");
            
            // Now lets chuck it into a Storage Queue.
            responseRoot.players.ForEach(p => _queueClient.SendMessage(Base64Encode(JsonSerializer.Serialize(p))));
            //responseRoot.players.ForEach(p => Console.WriteLine(p.player_id + " " + p.name));
        }

        

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
