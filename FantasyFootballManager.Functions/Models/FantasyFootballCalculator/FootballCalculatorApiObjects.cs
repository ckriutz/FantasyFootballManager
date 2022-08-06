using System.Collections.Generic;

namespace FantasyFootballManager.Functions.Models.FantasyFootballCalculator;

public class Root
{
    public string status { get; set; }
    public Meta meta { get; set; }
    public List<Player> players { get; set; }
}

public class Meta
{
    public string type { get; set; }
    public int teams { get; set; }
    public int rounds { get; set; }
    public int total_drafts { get; set; }
    public string start_date { get; set; }
    public string end_date { get; set; }
}

public class Player
{
    public int player_id { get; set; }
    public string name { get; set; }
    public string position { get; set; }
    public string team { get; set; }
    public double adp { get; set; }
    public string adp_formatted { get; set; }
    public int times_drafted { get; set; }
    public int high { get; set; }
    public int low { get; set; }
    public double stdev { get; set; }
    public int bye { get; set; }
}