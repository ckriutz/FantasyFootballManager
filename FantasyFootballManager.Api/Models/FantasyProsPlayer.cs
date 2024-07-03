using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace FantasyFootballManager.DataService.Models;        

public class FantasyProsPlayer
{
    [Key]
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; }
    public string SportsdataId { get; set; }
    public string PlayerTeamId { get; set; }
    public Team Team { get; set; } = new();
    public string PlayerPositionId { get; set; }
    public string PlayerPositions { get; set; }
    public string PlayerShortName { get; set; }
    public string PlayerEligibility { get; set; }
    public string PlayerYahooPositions { get; set; }
    public string PlayerPageUrl { get; set; }
    public string PlayerFilename { get; set; }
    public string PlayerSquareImageUrl { get; set; }
    public string PlayerImageUrl { get; set; }
    public string PlayerYahooId { get; set; }
    public string CbsPlayerId { get; set; }
    public string? PlayerByeWeek { get; set; }
    public double PlayerOwnedAvg { get; set; }
    public double PlayerOwnedEspn { get; set; }
    public int PlayerOwnedYahoo { get; set; }
    public int RankEcr { get; set; }
    public string RankMin { get; set; }
    public string RankMax { get; set; }
    public string RankAve { get; set; }
    public string RankStd { get; set; }
    public string PosRank { get; set; }
    public int Tier { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}