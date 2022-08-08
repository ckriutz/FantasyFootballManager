namespace FantasyFootballManager.Functions.Models.SportsDataIo;

public class SportsDataIOObject
{
    public string FantasyPlayerKey { get; set; }
    public int PlayerID { get; set; }                       // ✔
    public string Name { get; set; }                        // ✔
    public string Team { get; set; }                        // ✔
    public string Position { get; set; }                    // ✔
    public double AverageDraftPosition { get; set; }        // ✔
    public double AverageDraftPositionPPR { get; set; }
    public int? ByeWeek { get; set; }                        // ✔
    public double LastSeasonFantasyPoints { get; set; }     // ✔
    public double ProjectedFantasyPoints { get; set; }      // ✔
    public int AuctionValue { get; set; }                   // ✔
    public int AuctionValuePPR { get; set; }
    public int? AverageDraftPositionIDP { get; set; }        // ✔
    public object AverageDraftPositionRookie { get; set; }
    public object AverageDraftPositionDynasty { get; set; }
    public object AverageDraftPosition2QB { get; set; }
}