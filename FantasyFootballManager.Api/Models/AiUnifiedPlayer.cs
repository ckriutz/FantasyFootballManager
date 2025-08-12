namespace FantasyFootballManager.DataService.Models;

/// <summary>
/// Lightweight player data optimized for AI draft analysis to minimize token usage.
/// Contains only the essential fields needed for draft recommendations.
/// </summary>
public sealed record AiUnifiedPlayer
{
    public string PlayerId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string Team { get; init; } = string.Empty;
    public int? ByeWeek { get; init; }
    public int? RankEcr { get; init; }
    public double? ProjectedFantasyPoints { get; init; }
    public double? AverageDraftPositionPpr { get; init; }
    public int Age { get; init; }
    public bool IsThumbsUp { get; init; }
    public bool IsThumbsDown { get; init; }

    /// <summary>
    /// Creates an AI-optimized DTO from a full UnifiedPlayer object
    /// </summary>
    public static AiUnifiedPlayer FromUnifiedPlayer(UnifiedPlayer player)
    {
        return new AiUnifiedPlayer
        {
            PlayerId = player.PlayerId,
            Name = player.Name,
            Position = player.Position,
            Team = player.TeamAbbreviation,
            ByeWeek = player.ByeWeek,
            RankEcr = player.RankEcr,
            ProjectedFantasyPoints = player.ProjectedFantasyPoints,
            AverageDraftPositionPpr = player.AverageDraftPositionPpr,
            Age = player.Age,
            IsThumbsUp = player.IsThumbsUp,
            IsThumbsDown = player.IsThumbsDown
        };
    }
}