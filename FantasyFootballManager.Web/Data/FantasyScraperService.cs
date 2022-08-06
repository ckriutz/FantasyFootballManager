using System.Text.Json;

namespace FantasyFootballManager.Web.Data;

public class FantasyScraperService
{
    HttpClient _client;

    public FantasyScraperService(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
    }




    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}