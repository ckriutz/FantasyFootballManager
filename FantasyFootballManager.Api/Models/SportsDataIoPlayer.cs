using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace FantasyFootballManager.DataService.Models;

public class SportsDataIoPlayer
{
    [Key]
    public int Id { get; set; }
    
    public string FantasyPlayerKey { get; set; }

    public int PlayerID { get; set; }

    public string Name { get; set; }

    public Team? PlayerTeam { get; set; }

    public string Position { get; set; }

    public double? AverageDraftPosition { get; set; }

    public double? AverageDraftPositionPPR { get; set; }

    public int? ByeWeek { get; set; }

    public double? LastSeasonFantasyPoints { get; set; }

    public double? ProjectedFantasyPoints { get; set; }

    public double? AuctionValue { get; set; }

    public double? AuctionValuePPR { get; set; }

    public double? AverageDraftPositionIDP { get; set; }

    public double? AverageDraftPositionRookie { get; set; }

    public double? AverageDraftPositionDynasty { get; set; }

    public double? AverageDraftPosition2QB { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}