using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FantasyFootballManager.DataService.Models;

public interface IPlayersService
{
    // TODO: Adjust this so I can do it more generically.
    // I want to be able to get all the fields from all sources in one case.
    // The Existing case now.
    // And one where we only get the fields needed for AI.
    Task<IReadOnlyList<UnifiedPlayer>> GetRosterAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<UnifiedPlayer>> GetTopAvailableAsync(string userId, PlayersQueryOptions options, CancellationToken ct = default);
    Task<IReadOnlyList<UnifiedPlayer>> GetUnifiedPlayersAsync(string userId, CancellationToken ct = default);
    Task<UnifiedPlayer?> GetUnifiedPlayerAsync(string playerId, string userId, CancellationToken ct = default);
}

/// <summary>
/// Provides roster and available player lists leveraging the unified projection service.
/// Initial implementation focuses on simple rank-based selection; positional biasing can be layered later.
/// </summary>
public sealed class PlayersService : IPlayersService
{
    private readonly FantasyDbContext _db;
    private readonly IUnifiedPlayerService _unified;

    public PlayersService(FantasyDbContext db, IUnifiedPlayerService unified)
    {
        _db = db;
        _unified = unified;
    }

    public async Task<IReadOnlyList<UnifiedPlayer>> GetUnifiedPlayersAsync(CancellationToken ct = default)
    {
        // TODO: Adjust this to take on a PlayersQueryOptions
        // Fetch all unified players, limited to a reasonable count for performance
        var fetchCount = 250; // Adjust as needed
        return await _unified.GetUnifiedPlayersAsync(fetchCount, null, ct);
    } 

    public async Task<IReadOnlyList<UnifiedPlayer>> GetUnifiedPlayersAsync(string userId, CancellationToken ct = default)
    {
        // TODO: Adjust this to take on a PlayersQueryOptions
        // Fetch all unified players, limited to a reasonable count for performance
        var fetchCount = 250; // Adjust as needed
        return await _unified.GetUnifiedPlayersAsync(fetchCount, userId, ct);
    }

    public async Task<UnifiedPlayer?> GetUnifiedPlayerAsync(string playerId, string userId, CancellationToken ct = default)
    {
        // Fetch a single unified player by Sleeper ID
        return await _unified.GetUnifiedPlayerAsync(playerId, userId, ct);
    }

    public async Task<IReadOnlyList<UnifiedPlayer>> GetRosterAsync(string userId, CancellationToken ct = default)
    {
        var rosterSleeperIds = await _db.FantasyActivities.AsNoTracking()
            .Where(a => a.User == userId && a.IsDraftedOnMyTeam)
            .Select(a => a.PlayerId.ToString())
            .ToListAsync(ct);

        if (rosterSleeperIds.Count == 0)
            return Array.Empty<UnifiedPlayer>();

        var list = new List<UnifiedPlayer>(rosterSleeperIds.Count);
        foreach (var id in rosterSleeperIds)
        {
            var unified = await _unified.GetUnifiedPlayerAsync(id, userId, ct);
            if (unified != null)
                list.Add(unified);
        }
        return list;
    }

    public async Task<IReadOnlyList<UnifiedPlayer>> GetTopAvailableAsync(string userId, PlayersQueryOptions options, CancellationToken ct = default)
    {
        options = options.Normalize();

        // Get all players drafted by anyone (both my team and other teams)
        var allDraftedSleeperIds = await _db.FantasyActivities.AsNoTracking()
            .Where(a => a.IsDraftedOnMyTeam || a.IsDraftedOnOtherTeam)
            .Select(a => a.PlayerId.ToString())
            .Distinct()
            .ToListAsync(ct);

        var fetchCount = Math.Min(options.HardCap * 2, 200);
        var ranked = await _unified.GetUnifiedPlayersAsync(fetchCount, userId, ct);

        var available = ranked.Where(p => !allDraftedSleeperIds.Contains(p.PlayerId)).ToList();

        if (!options.IncludeK)
            available = available.Where(p => !string.Equals(p.Position, "K", StringComparison.OrdinalIgnoreCase)).ToList();
        if (!options.IncludeDst)
            available = available.Where(p => !string.Equals(p.Position, "DST", StringComparison.OrdinalIgnoreCase) && !string.Equals(p.Position, "DEF", StringComparison.OrdinalIgnoreCase)).ToList();

        var trimmed = available.Take(options.OverallLimit).ToList();

        if (options.BiasToNeeds)
        {
            trimmed = BalancePositions(trimmed, options.PerPositionLimit);
        }

        if (trimmed.Count > options.HardCap)
            trimmed = trimmed.Take(options.HardCap).ToList();

        return trimmed;
    }

    private static List<UnifiedPlayer> BalancePositions(List<UnifiedPlayer> players, int perPositionLimit)
    {
        var grouped = players.GroupBy(p => p.Position ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Take(perPositionLimit).ToList());
        var result = new List<UnifiedPlayer>();
        var maxGroupSize = grouped.Values.Max(g => g.Count);
        for (int i = 0; i < maxGroupSize; i++)
        {
            foreach (var kvp in grouped)
            {
                if (i < kvp.Value.Count)
                    result.Add(kvp.Value[i]);
            }
        }
        return result;
    }
}
