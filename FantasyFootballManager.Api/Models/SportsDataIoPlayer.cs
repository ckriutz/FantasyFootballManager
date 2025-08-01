using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballManager.DataService.Models;

public class SportsDataIoPlayer
{
    [Key]
    public int Id { get; set; }

    [JsonPropertyName("FantasyPlayerKey")]
    public string FantasyPlayerKey { get; set; } = string.Empty;

    [JsonPropertyName("PlayerID")]
    public int PlayerID { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;

    [NotMapped]
    [JsonPropertyName("Team")]
    public string? TeamAbbreviation { get; set; }

    public Team? PlayerTeam { get; set; }

    [JsonPropertyName("Position")]
    public string Position { get; set; } = string.Empty;

    [JsonPropertyName("AverageDraftPosition")]
    public double? AverageDraftPosition { get; set; }

    [JsonPropertyName("AverageDraftPositionPPR")]
    public double? AverageDraftPositionPPR { get; set; }

    [JsonPropertyName("ByeWeek")]
    public int? ByeWeek { get; set; }

    [JsonPropertyName("LastSeasonFantasyPoints")]
    public double? LastSeasonFantasyPoints { get; set; }

    [JsonPropertyName("ProjectedFantasyPoints")]
    public double? ProjectedFantasyPoints { get; set; }

    [JsonPropertyName("AuctionValue")]
    public double? AuctionValue { get; set; }

    [JsonPropertyName("AuctionValuePPR")]
    public double? AuctionValuePPR { get; set; }

    [JsonPropertyName("AverageDraftPositionIDP")]
    public double? AverageDraftPositionIDP { get; set; }

    [JsonPropertyName("AverageDraftPositionRookie")]
    public double? AverageDraftPositionRookie { get; set; }

    [JsonPropertyName("AverageDraftPositionDynasty")]
    public double? AverageDraftPositionDynasty { get; set; }

    [JsonPropertyName("AverageDraftPosition2QB")]
    public double? AverageDraftPosition2QB { get; set; }

    public System.DateTime LastUpdated { get; set; } = System.DateTime.UtcNow;
}