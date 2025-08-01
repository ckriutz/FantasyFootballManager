namespace FantasyFootballManager.DataService.Models;

public class User
{
    // From Auth0, we're getting this as the sub value.
    public int Id { get; set; }
    public string Auth0Id { get; set; } = string.Empty;
    public string YahooUsername { get; set; } = string.Empty;
    public string YahooLeagueId { get; set; } = string.Empty;
    public string EspnUsername { get; set; } = string.Empty;
    public string EspnLeagueId { get; set; } = string.Empty;
    public string SleeperUsername { get; set; } = string.Empty;
    public string SleeperLeagueId { get; set; } = string.Empty;
}
