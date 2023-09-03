using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Note. None of these items are going into the database, just into Redis for now.

namespace FantasyFootballManager.DataService.Models; 
public class SleeperDraftResult
{
    [JsonPropertyName("round")]
    public int Round { get; set; }

    [JsonPropertyName("roster_id")]
    public int RosterId { get; set; }

    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; }

    [JsonPropertyName("picked_by")]
    public string PickedBy { get; set; }

    [JsonPropertyName("pick_no")]
    public int PickNo { get; set; }

    [JsonPropertyName("metadata")]
    public SleeperDraftMetadata Metadata { get; set; }

    [JsonPropertyName("is_keeper")]
    public bool? IsKeeper { get; set; }

    [JsonPropertyName("draft_slot")]
    public int DraftSlot { get; set; }

    [JsonPropertyName("draft_id")]
    public string DraftId { get; set; }
}

public class SleeperDraftMetadata
{
    [JsonPropertyName("years_exp")]
    public string YearsExp { get; set; }

    [JsonPropertyName("team")]
    public string Team { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("sport")]
    public string Sport { get; set; }

    [JsonPropertyName("position")]
    public string Position { get; set; }

    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }

    [JsonPropertyName("news_updated")]
    public string NewsUpdated { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("injury_status")]
    public string InjuryStatus { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
}

