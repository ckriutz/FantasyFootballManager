namespace FantasyFootballManager.DataService.Models;

/// <summary>
/// Options controlling how available players are selected for draft analysis.
/// Keep defaults conservative to limit token usage when sending to AI.
/// </summary>
public sealed record AvailablePlayersQueryOptions(
    int OverallLimit = 40,        // Base overall cap before positional supplements
    int PerPositionLimit = 12,    // Max supplemental pulls per position
    bool IncludeK = false,        // Include Kickers
    bool IncludeDst = false,      // Include DST / Defense
    bool BiasToNeeds = true,      // Add extra candidates for deficit positions
    int NeedsMultiplier = 4,      // Candidates per deficit slot (deficit 2 -> 8)
    int HardCap = 60              // Absolute upper bound after merging
)
{
    /// <summary>
    /// Returns a sanitized copy with enforced minimum/maximum boundaries.
    /// </summary>
    public AvailablePlayersQueryOptions Normalize()
    {
        var overall = OverallLimit <= 0 ? 40 : OverallLimit;
        var perPos = PerPositionLimit <= 0 ? 8 : PerPositionLimit;
        var needsMult = NeedsMultiplier <= 0 ? 3 : NeedsMultiplier;
        var hard = HardCap < overall ? overall : HardCap;
        if (hard > 120) hard = 120; // safety upper bound
        return this with
        {
            OverallLimit = overall,
            PerPositionLimit = perPos,
            NeedsMultiplier = needsMult,
            HardCap = hard
        };
    }
}
