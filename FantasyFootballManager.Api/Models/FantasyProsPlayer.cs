using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace FantasyFootballManager.DataService.Models;        

public class FantasyProsReturnObject
{
    [JsonPropertyName("sport")]
    public string Sport { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("ranking_type_name")]
    public string RankingTypeName { get; set; } = string.Empty;

    [JsonPropertyName("year")]
    public string Year { get; set; } = string.Empty;

    [JsonPropertyName("week")]
    public string Week { get; set; } = string.Empty;

    [JsonPropertyName("position_id")]
    public string PositionId { get; set; } = string.Empty;

    [JsonPropertyName("scoring")]
    public string Scoring { get; set; } = string.Empty;

    [JsonPropertyName("filters")]
    public string Filters { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("total_experts")]
    public int TotalExperts { get; set; }
    
    [JsonPropertyName("players")]
    public List<FantasyProsPlayer> Players { get; set; } = new();
}

public class FantasyProsPlayer
{
    [Key]
    public int Id { get; set; }

    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("sportsdata_id")]
    public string SportsdataId { get; set; } = string.Empty;

    [JsonPropertyName("player_team_id")]
    public string PlayerTeamId { get; set; } = string.Empty;

    public Team Team { get; set; } = new();

    [JsonPropertyName("player_position_id")]
    public string PlayerPositionId { get; set; } = string.Empty;

    [JsonPropertyName("player_positions")]
    public string PlayerPositions { get; set; } = string.Empty;

    [JsonPropertyName("player_short_name")]
    public string PlayerShortName { get; set; } = string.Empty;

    [JsonPropertyName("player_eligibility")]
    public string PlayerEligibility { get; set; } = string.Empty;

    [JsonPropertyName("player_yahoo_positions")]
    public string PlayerYahooPositions { get; set; } = string.Empty;

    [JsonPropertyName("player_page_url")]
    public string PlayerPageUrl { get; set; } = string.Empty;

    [JsonPropertyName("player_filename")]
    public string PlayerFilename { get; set; } = string.Empty;

    [JsonPropertyName("player_square_image_url")]
    public string PlayerSquareImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("player_image_url")]
    public string PlayerImageUrl { get; set; } = string.Empty;

    [JsonPropertyName("player_yahoo_id")]
    public string PlayerYahooId { get; set; } = string.Empty;

    [JsonPropertyName("cbs_player_id")]
    public string CbsPlayerId { get; set; } = string.Empty;

    [JsonPropertyName("player_bye_week")]
    public string? PlayerByeWeek { get; set; }

    [JsonPropertyName("player_owned_avg")]
    public double PlayerOwnedAvg { get; set; }

    [JsonPropertyName("player_owned_espn")]
    public double PlayerOwnedEspn { get; set; }

    [JsonPropertyName("player_owned_yahoo")]
    public int PlayerOwnedYahoo { get; set; }

    [NotMapped]
    [JsonPropertyName("player_ecr_delta")]
    public object PlayerEcrDelta { get; set; }

    [JsonPropertyName("rank_ecr")]
    public int RankEcr { get; set; }

    [JsonPropertyName("rank_min")]
    public string RankMin { get; set; } = string.Empty;

    [JsonPropertyName("rank_max")]
    public string RankMax { get; set; } = string.Empty;

    [JsonPropertyName("rank_ave")]
    public string RankAve { get; set; } = string.Empty;

    [JsonPropertyName("rank_std")]
    public string RankStd { get; set; } = string.Empty;

    [JsonPropertyName("pos_rank")]
    public string PosRank { get; set; } = string.Empty;

    [JsonPropertyName("tier")]
    public int Tier { get; set; }

    public System.DateTime LastUpdated { get; set; } = System.DateTime.UtcNow;
}