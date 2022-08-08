using System;
using System.Text.Json.Serialization;

namespace FantasyFootballManager.Functions.Models;

public class FootballPlayer
{
    /*************************
        KEYS
    *************************/

    //Primary Key for the Database.
    [JsonPropertyName("id")]
    public int Id { get; set; }

    // The key indicating the id of the Fantasy Pros Player
    [JsonPropertyName("fantasyProsId")]
    public int FantasyProsId { get; set; }

    // The key indicating the id for the football calculator player.
    [JsonPropertyName("footballCalculatorId")]
    public int FootballCalculatorId { get; set; }

    // The guid that represents the SportsDataIO Id of the player.
    [JsonPropertyName("sportsDataIOId")]
    public int SportsDataIOId { get; set; }

    /*************************
    *   PLAYER METADATA
    *************************/

    // From SportsData.IO
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    // From SportsData.IO
    [JsonPropertyName("position")]
    public string Position { get; set; }
    
    // From SportsData.IO
    [JsonPropertyName("team")]
    public string Team { get; set; }       
    
    // From SportsData.IO
    [JsonPropertyName("bye")]
    public int Bye { get; set; }
      
    // From FFantasyPros
    [JsonPropertyName("playerHeadshotURL")]
    public string PlayerHeadshotURL { get; set; }

    /*************************
    *   RANKINGS
    *************************/
    
    // From FantasyFootball Calculator
    [JsonPropertyName("ffcRank")]
    public int FFCRank { get; set; }
    
    // From FantasyPros
    [JsonPropertyName("fantasyProsRank")]
    public int FantasyProsRank { get; set; }

    [JsonIgnore]
    public double Rank
    {
        // FantasyPros is weighted higher than the other ones.
        get{ return (FFCRank * .5) + (FantasyProsRank * .5);  }
    }

    // From SportsData.IO
    [JsonPropertyName("averageDraftPositionSportsData")]
    public double AverageDraftPositionSportsData { get; set; }

    // From Football Calculator
    [JsonPropertyName("averageDraftPositionFCalculator")]
    public double AverageDraftPositionFCalculator { get; set; }

    [JsonIgnore]
    public double ADP
    {
        // FantasyPros is weighted higher than the other ones.
        get{ return (AverageDraftPositionSportsData * .55) + (AverageDraftPositionFCalculator * .45);  }
    }

    // From SportsData.IO
    [JsonPropertyName("lastSeasonFantasyPoints")]
    public double LastSeasonFantasyPoints { get; set; }
    
    // From SportsData.IO
    [JsonPropertyName("projectedFantasyPoints")]
    public double ProjectedFantasyPoints { get; set; }

    // From SportsData.IO
    [JsonPropertyName("auctionValue")]
    public int AuctionValue { get; set; }

    // From FantasyPros
    [JsonPropertyName("tier")]
    public int Tier { get; set; }// From FantasyPros


    /*************************
    * Fantasy Data
    *************************/
    [JsonPropertyName("isAvailable")]
    public Boolean IsAvailable { get; set; }

    [JsonPropertyName("isOnMyTeam")]
    public Boolean IsOnMyTeam { get; set; }

    [JsonPropertyName("lastUpdatedSportsDataIO")]
    public DateTime LastUpdatedSportsDataIO { get; set; }

    [JsonPropertyName("lastUpdatedFantasyPros")]
    public DateTime LastUpdatedFantasyPros { get; set; }

    [JsonPropertyName("lastUpdatedLineups")]
    public DateTime LastUpdatedLineups { get; set; }

    [JsonPropertyName("lastUpdatedFFootballCalculator")]
    public DateTime LastUpdatedFFootballCalculator { get; set; }
}