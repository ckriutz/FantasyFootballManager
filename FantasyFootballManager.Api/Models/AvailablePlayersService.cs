using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.DataService.Models;

public interface IAvailablePlayersService
{
    Task<IReadOnlyList<UnifiedPlayerProjection>> GetRosterAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<UnifiedPlayerProjection>> GetTopAvailableAsync(string userId, AvailablePlayersQueryOptions options, CancellationToken ct = default);
}

/// <summary>
/// Provides roster and available player lists leveraging the unified projection service.
/// Initial implementation focuses on simple rank-based selection; positional biasing can be layered later.
/// </summary>
public sealed class AvailablePlayersService : IAvailablePlayersService
{
    private readonly FantasyDbContext _db;
    private readonly IUnifiedPlayerProjectionService _unified;

    public AvailablePlayersService(FantasyDbContext db, IUnifiedPlayerProjectionService unified)
    {
        _db = db;
        _unified = unified;
    }

    public async Task<IReadOnlyList<UnifiedPlayerProjection>> GetRosterAsync(string userId, CancellationToken ct = default)
    {
        var rosterSleeperIds = await _db.FantasyActivities.AsNoTracking()
            .Where(a => a.User == userId && a.IsDraftedOnMyTeam)
            .Select(a => a.PlayerId.ToString())
            .ToListAsync(ct);

        if (rosterSleeperIds.Count == 0)
            return Array.Empty<UnifiedPlayerProjection>();

        var list = new List<UnifiedPlayerProjection>(rosterSleeperIds.Count);
        foreach (var id in rosterSleeperIds)
        {
            var unified = await _unified.GetUnifiedPlayerAsync(id, ct);
            if (unified != null)
                list.Add(unified);
        }
        return list;
    }

    public async Task<IReadOnlyList<UnifiedPlayerProjection>> GetTopAvailableAsync(string userId, AvailablePlayersQueryOptions options, CancellationToken ct = default)
    {
        options = options.Normalize();

        var rosterSleeperIds = await _db.FantasyActivities.AsNoTracking()
            .Where(a => a.User == userId && a.IsDraftedOnMyTeam)
            .Select(a => a.PlayerId.ToString())
            .ToListAsync(ct);

        var fetchCount = Math.Min(options.HardCap * 2, 200);
        var ranked = await _unified.GetUnifiedPlayersAsync(fetchCount, ct);

        var available = ranked.Where(p => !rosterSleeperIds.Contains(p.SleeperPlayerId)).ToList();

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

    private static List<UnifiedPlayerProjection> BalancePositions(List<UnifiedPlayerProjection> players, int perPositionLimit)
    {
        var grouped = players.GroupBy(p => p.Position ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Take(perPositionLimit).ToList());
        var result = new List<UnifiedPlayerProjection>();
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
