using System.Text.Json;

namespace FantasyFootballManager.Web.Data;

public class FootballPlayerService
{
    // I basically took this right from: https://docs.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-6.0&pivots=server
    HttpClient _client;

    public FootballPlayerService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
    }

    public async Task<List<FootballPlayer>> GetAllFootballPlayersAsync()
    {
        var players = new List<FootballPlayer>();
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/players/");
        var response = await _client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            players = await JsonSerializer.DeserializeAsync<List<Data.FootballPlayer>>(responseStream);
        }

        return players;
    }

    public async Task<List<string>> GetAllPositionsAsync()
    {
        List<string> positions = new List<string>();
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:7071/api/positions/");

        var response = await _client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            using var responseStream = await response.Content.ReadAsStreamAsync();
            positions = await JsonSerializer.DeserializeAsync<List<string>>(responseStream);
            if(!positions.Contains("All"))
            {
                positions.Add("All");
            }
        }

        return positions;
    }

    public async Task<bool> UpdatePlayer(Data.FootballPlayer player)
    {
        var playerResponse = await _client.PutAsJsonAsync<Data.FootballPlayer>("http://localhost:7071/api/player/", player);
        return playerResponse.IsSuccessStatusCode;
    }

}