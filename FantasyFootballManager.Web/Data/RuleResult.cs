namespace FantasyFootballManager.Web.Data;

public class RuleResult
{
    // This is how we take the results, and turn it into a suggestion.
    public int Importance { get; set; }
    public string Position { get; set; }
    public string Message { get; set; }
    public FootballPlayer suggestedPlayer { get; set; }

}