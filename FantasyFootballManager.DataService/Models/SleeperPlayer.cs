using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FantasyFootballManager.DataService.Models;

public class SleeperPlayer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [JsonPropertyName("player_id")]
    public required string PlayerId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("birth_date")]
    public DateTime? Birthdate { get; set; }

    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [NotMapped]
    [JsonPropertyName("practice_participation")]
    public object? PracticeParticipation { get; set; }

    [NotMapped]
    [JsonPropertyName("birth_state")]
    public object? BirthState { get; set; }

    [JsonPropertyName("yahoo_id")]
    public int? YahooId { get; set; }

    [NotMapped]
    [JsonPropertyName("pandascore_id")]
    public object? PandascoreId { get; set; }

    [JsonPropertyName("stats_id")]
    public int? StatsId { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [NotMapped]
    [JsonPropertyName("fantasy_positions")]
    public List<string> FantasyPositions { get; set; } = new();

    [JsonPropertyName("search_full_name")]
    public string? SearchFullName { get; set; }

    [JsonPropertyName("weight")]
    public string? Weight { get; set; }

    [NotMapped]
    [JsonPropertyName("birth_city")]
    public object? BirthCity { get; set; }

    [JsonPropertyName("search_first_name")]
    public string? SearchFirstName { get; set; }

    [JsonPropertyName("injury_body_part")]
    public string? InjuryBodyPart { get; set; }

    [JsonPropertyName("fantasy_data_id")]
    public int? FantasyDataId { get; set; }

    [JsonPropertyName("years_exp")]
    public int? YearsExp { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("rotoworld_id")]
    public int? RotoworldId { get; set; }

    [JsonPropertyName("gsis_id")]
    public string? GsisId { get; set; }

    [JsonPropertyName("news_updated")]
    public long? NewsUpdated { get; set; }

    [JsonPropertyName("espn_id")]
    public int? EspnId { get; set; }

    [JsonPropertyName("hashtag")]
    public string? Hashtag { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("height")]
    public string? Height { get; set; }

    [NotMapped]
    [JsonPropertyName("team")]
    public string? TeamAbbreviation { get; set; }

    public Team? Team { get; set; }

    [JsonPropertyName("depth_chart_position")]
    public string? DepthChartPosition { get; set; }

    [JsonPropertyName("search_rank")]
    public int? SearchRank { get; set; }

    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("high_school")]
    public string? HighSchool { get; set; }

    [JsonPropertyName("depth_chart_order")]
    public int? DepthChartOrder { get; set; }

    [JsonPropertyName("rotowire_id")]
    public int? RotowireId { get; set; }

    [NotMapped]
    [JsonPropertyName("injury_start_date")]
    public DateTime? injuryStartDate { get; set; }

    [JsonPropertyName("college")]
    public string? College { get; set; }

    [JsonPropertyName("injury_status")]
    public string? InjuryStatus { get; set; }

    [NotMapped]
    [JsonPropertyName("sport")]
    public string? Sport { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("active")]
    public bool IsActive { get; set; } = false;

    [JsonPropertyName("swish_id")]
    public int? SwishId { get; set; }

    [JsonPropertyName("search_last_name")]
    public string? SearchLastName { get; set; }

    [NotMapped]
    [JsonPropertyName("practice_description")]
    public string? PracticeDescription { get; set; }

    [NotMapped]
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    [JsonPropertyName("injury_notes")]
    public string? InjuryNotes { get; set; }

    [JsonPropertyName("birth_country")]
    public string? BirthCountry { get; set; }

    [JsonPropertyName("sportradar_id")]
    public string? SportRadarId { get; set; }

    public DateTime LastUpdated { get; set; }
}