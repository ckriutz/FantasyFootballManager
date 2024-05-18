
// This object represents the combination of all the data in a way that we can use.
// We are not going to write this to the database, but instead, we will push this into redis.

using System;
using Redis.OM.Modeling;

namespace FantasyFootballManager.DataService.Models;

[Document(IndexName = "fantasyplayers-idx", StorageType = StorageType.Json)]
public class FantasyPlayer
{
    public FantasyPlayer() { }

    // These fields are mostly identifiers, and mostly come from Sleeper.
    [RedisIdField][Indexed] public string SleeperId { get; set; } = string.Empty;       // From Sleeper
    [Indexed] public string? SportRadarId { get; set; } = string.Empty;   // From Sleeper
    public string FirstName { get; set; } = string.Empty;       // From Sleeper
    public string LastName { get; set; } = string.Empty;        // From Sleeper
    [Indexed] public string FullName { get; set; } = string.Empty;        // From Sleeper
    [Indexed] public string Status { get; set; } = "Inactive";            // From Sleeper, default to inactive
    public string InjuryStatus { get; set; } = string.Empty;    // From Sleeper
    [Indexed] public string Position { get; set; } = string.Empty;        // From Sleeper
    public int? TeamId { get; set; }                            // From Sleeper
    public string TeamName { get; set; } = string.Empty;        // From Sleeper
    [Indexed] public int? YahooId { get; set; }                           // From Sleeper, joins to FantasyPros player using YahooId.
    public int? SearchRank { get; set; }                        // From Sleeper, is basically a rank of the player according to Sleeper.
    public string InjuryBodyPart { get; set; } = string.Empty;  // From Sleeper
    public int? Age { get; set; } = 0;                          // From Sleeper
    public int? DepthChartOrder { get; set; }                   // From Sleeper
    public string? College { get; set; } = string.Empty;        // From Sleeper
    public DateTime LastUpdatedSleeper { get; set; }            // From Sleeper

    // These fields are mostly from SportsData.io
    [Indexed] public string SportsDataIoKey { get; set; } = string.Empty;    // From SportsData.io
    public double? AverageDraftPosition { get; set; }              // From SportsData.io
    public double? AverageDraftPositionPPR { get; set; }           // From SportsData.io
    public double? ByeWeek { get; set; }                           // From SportsData.io
    public double? LastSeasonFantasyPoints { get; set; }           // From SportsData.io
    public double? ProjectedFantasyPoints { get; set; }            // From SportsData.io
    public double? AuctionValue { get; set; }                      // From SportsData.io
    public double? AuctionValuePPR { get; set; }                   // From SportsData.io
    public double? AverageDraftPositionIDP { get; set; }           // From SportsData.io
    public double? AverageDraftPositionRookie { get; set; }        // From SportsData.io
    public double? AverageDraftPositionDynasty { get; set; }       // From SportsData.io
    public double? AverageDraftPosition2QB { get; set; }           // From SportsData.io
    public DateTime LastUpdatedSportsDataIo { get; set; }          // From SportsData.io

    // These fields are mostly from FantasyPros
    [Indexed] public int FantasyProsPlayerId { get; set; }                   // From FantasyPros
    [Indexed] public string SportsdataId { get; set; }                       // From FantasyPros
    public string PlayerSquareImageUrl { get; set; }               // From FantasyPros
    public string PlayerImageUrl { get; set; }                     // From FantasyPros
    public double PlayerOwnedAvg { get; set; }                     // From FantasyPros
    public double PlayerOwnedEspn { get; set; }                    // From FantasyPros
    public int PlayerOwnedYahoo { get; set; }                      // From FantasyPros
    [Indexed] public int RankEcr { get; set; }                               // From FantasyPros
    public string RankMin { get; set; }                            // From FantasyPros
    public string RankMax { get; set; }                            // From FantasyPros
    public string RankAve { get; set; }                            // From FantasyPros
    public string RankStd { get; set; }                            // From FantasyPros
    public string PosRank { get; set; }                            // From FantasyPros
    [Indexed] public int Tier { get; set; }                                  // From FantasyPros
    public DateTime LastUpdatedFantasyPros { get; set; }           // From FantasyPros

    // These are the operational items for the FantasyPlayer
    public bool IsThumbsUp { get; set; } = false;                  // This is a flag that the user can set to indicate they like this player.
    public bool IsThumbsDown { get; set; } = false;                // This is a flag that the user can set to indicate they don't like this player.
    [Indexed] public bool IsTaken { get; set; } = false;                     // This is a flag that the user can set to indicate someone else has taken this player.
    [Indexed] public bool IsOnMyTeam { get; set; } = false;                  // This is a flag that the user can set to indicate they have taken this player.

    // These are draft result information from Sleeper
    public string PickedBy { get; set; } = string.Empty;           // From Sleeper Draft
    public int? PickNumber { get; set; }                           // From Sleeper Draft
    public int? PickRound { get; set; }                            // From Sleeper Draft
}