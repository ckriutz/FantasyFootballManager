namespace FantasyFootballManager.DataService.Models;

public class User
{
    // From Auth0, we're getting this as the sub value.
    public int Id { get; set; }
    public string YahooUsername { get; set; } = string.Empty;
    public string YahooLeagueId { get; set; } = string.Empty;
}
