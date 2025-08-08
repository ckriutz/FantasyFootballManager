using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.DataService.Models;

/// <summary>
/// Unified minimal player projection combining Sleeper (canonical id), FantasyPros (rank/bye), and SportsDataIO (projections/ADP).
/// This DTO keeps only fields needed for AI draft analysis.
/// </summary>
public sealed record UnifiedPlayerProjection(
    string SleeperPlayerId,
    string Name,
    string Position,
    string TeamAbbreviation,
    int? ByeWeek,
    int? RankEcr,
    double? ProjectedFantasyPoints,
    double? AverageDraftPosition,
    double? AverageDraftPositionPpr,
    string MatchQuality // "ExactId", "ExactName", "NameFallback", "SleeperOnly"
);

public interface IUnifiedPlayerProjectionService
{
    Task<IReadOnlyList<UnifiedPlayerProjection>> GetUnifiedPlayersAsync(int? topNRanked = null, CancellationToken ct = default);
    Task<UnifiedPlayerProjection?> GetUnifiedPlayerAsync(string sleeperPlayerId, CancellationToken ct = default);
}

public sealed class UnifiedPlayerProjectionService : IUnifiedPlayerProjectionService
{
    private readonly FantasyDbContext _db;

    public UnifiedPlayerProjectionService(FantasyDbContext db) => _db = db;

    public async Task<IReadOnlyList<UnifiedPlayerProjection>> GetUnifiedPlayersAsync(int? topNRanked = null, CancellationToken ct = default)
    {
        var sleeperTask = _db.SleeperPlayers.AsNoTracking()
            .Select(p => new { p.PlayerId, p.FullName, p.Position, SportRadarId = p.SportRadarId, Team = p.Team != null ? p.Team.Abbreviation : null })
            .ToListAsync(ct);
        var fantasyProsTask = _db.FantasyProsPlayers.AsNoTracking()
            .Select(p => new { p.SportsdataId, p.PlayerName, p.PlayerPositionId, p.PlayerTeamId, p.PlayerByeWeek, p.RankEcr })
            .ToListAsync(ct);
        var sportsDataTask = _db.SportsDataIoPlayers.AsNoTracking()
            .Select(p => new { p.Name, p.Position, p.ByeWeek, p.ProjectedFantasyPoints, p.AverageDraftPosition, p.AverageDraftPositionPPR, Team = p.PlayerTeam != null ? p.PlayerTeam.Abbreviation : null })
            .ToListAsync(ct);

        await Task.WhenAll(sleeperTask, fantasyProsTask, sportsDataTask);

        var sleeper = sleeperTask.Result;
        var fantasy = fantasyProsTask.Result;
        var sports = sportsDataTask.Result;

        var fantasyBySportsDataId = fantasy
            .Where(f => !string.IsNullOrWhiteSpace(f.SportsdataId))
            .GroupBy(f => f.SportsdataId!)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.RankEcr).First());

        var fantasyByName = fantasy
            .Where(f => !string.IsNullOrWhiteSpace(f.PlayerName))
            .GroupBy(f => NormalizeName(f.PlayerName))
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.RankEcr).First());

        var sportsByName = sports
            .Where(s => !string.IsNullOrWhiteSpace(s.Name))
            .GroupBy(s => NormalizeName(s.Name))
            .ToDictionary(g => g.Key, g => g.First());

        var unified = new List<UnifiedPlayerProjection>(sleeper.Count);

        foreach (var sp in sleeper)
        {
            var name = sp.FullName ?? string.Empty;
            var position = sp.Position ?? string.Empty;
            var team = sp.Team ?? string.Empty;
            int? bye = null; int? rank = null;
            double? proj = null; double? adp = null; double? adpPpr = null;
            var matchQuality = "SleeperOnly";

            if (!string.IsNullOrWhiteSpace(sp.SportRadarId) && fantasyBySportsDataId.TryGetValue(sp.SportRadarId, out var fpById))
            {
                EnrichFromFantasyPros(fpById, ref position, ref team, ref bye, ref rank);
                matchQuality = "ExactId";
            }
            else
            {
                var nameKey = NormalizeName(name);
                if (fantasyByName.TryGetValue(nameKey, out var fpByName))
                {
                    EnrichFromFantasyPros(fpByName, ref position, ref team, ref bye, ref rank);
                    matchQuality = "ExactName";
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var key = NormalizeName(name);
                if (sportsByName.TryGetValue(key, out var sd))
                {
                    proj = sd.ProjectedFantasyPoints;
                    if (sd.ByeWeek.HasValue && bye == null) bye = sd.ByeWeek;
                    if (string.IsNullOrWhiteSpace(position)) position = sd.Position;
                    if (string.IsNullOrWhiteSpace(team)) team = sd.Team ?? team;
                    if (sd.AverageDraftPosition.HasValue) adp = sd.AverageDraftPosition;
                    if (sd.AverageDraftPositionPPR.HasValue) adpPpr = sd.AverageDraftPositionPPR;
                    if (matchQuality == "SleeperOnly") matchQuality = "NameFallback";
                }
            }

            if (string.IsNullOrWhiteSpace(name)) name = sp.PlayerId;
            if (string.IsNullOrWhiteSpace(position)) position = "?";

            unified.Add(new UnifiedPlayerProjection(
                sp.PlayerId,
                name,
                position,
                team,
                bye,
                rank,
                proj,
                adp,
                adpPpr,
                matchQuality));
        }

        if (topNRanked.HasValue)
        {
            unified = unified
                .OrderBy(p => p.RankEcr == null)
                .ThenBy(p => p.RankEcr)
                .ThenByDescending(p => p.ProjectedFantasyPoints)
                .ThenBy(p => p.Name)
                .Take(topNRanked.Value)
                .ToList();
        }

        return unified;
    }

    public async Task<UnifiedPlayerProjection?> GetUnifiedPlayerAsync(string sleeperPlayerId, CancellationToken ct = default)
    {
        var basePlayer = await _db.SleeperPlayers.AsNoTracking()
            .Where(p => p.PlayerId == sleeperPlayerId)
            .Select(p => new { p.PlayerId, p.FullName, p.Position, SportRadarId = p.SportRadarId, Team = p.Team != null ? p.Team.Abbreviation : null })
            .FirstOrDefaultAsync(ct);
        if (basePlayer == null) return null;

        int? bye = null; int? rank = null; double? proj = null; double? adp = null; double? adpPpr = null; string matchQuality = "SleeperOnly"; string position = basePlayer.Position ?? string.Empty; string team = basePlayer.Team ?? string.Empty; string name = basePlayer.FullName ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(basePlayer.SportRadarId))
        {
            var fp = await _db.FantasyProsPlayers.AsNoTracking()
                .Where(f => f.SportsdataId == basePlayer.SportRadarId)
                .OrderBy(f => f.RankEcr)
                .FirstOrDefaultAsync(ct);
            if (fp != null)
            {
                EnrichFromFantasyPros(fp, ref position, ref team, ref bye, ref rank);
                matchQuality = "ExactId";
            }
        }
        if (matchQuality == "SleeperOnly")
        {
            var key = NormalizeName(name);
            var fp = await _db.FantasyProsPlayers.AsNoTracking()
                .Where(f => NormalizeName(f.PlayerName) == key)
                .OrderBy(f => f.RankEcr)
                .FirstOrDefaultAsync(ct);
            if (fp != null)
            {
                EnrichFromFantasyPros(fp, ref position, ref team, ref bye, ref rank);
                matchQuality = "ExactName";
            }
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var key = NormalizeName(name);
            var sd = await _db.SportsDataIoPlayers.AsNoTracking()
                .Where(s => NormalizeName(s.Name) == key)
                .FirstOrDefaultAsync(ct);
            if (sd != null)
            {
                proj = sd.ProjectedFantasyPoints;
                if (sd.ByeWeek.HasValue && bye == null) bye = sd.ByeWeek;
                if (string.IsNullOrWhiteSpace(position)) position = sd.Position;
                if (string.IsNullOrWhiteSpace(team) && sd.PlayerTeam != null) team = sd.PlayerTeam.Abbreviation;
                if (sd.AverageDraftPosition.HasValue) adp = sd.AverageDraftPosition;
                if (sd.AverageDraftPositionPPR.HasValue) adpPpr = sd.AverageDraftPositionPPR;
                if (matchQuality == "SleeperOnly") matchQuality = "NameFallback";
            }
        }

        if (string.IsNullOrWhiteSpace(name)) name = sleeperPlayerId;
        if (string.IsNullOrWhiteSpace(position)) position = "?";

        return new UnifiedPlayerProjection(
            sleeperPlayerId,
            name,
            position,
            team,
            bye,
            rank,
            proj,
            adp,
            adpPpr,
            matchQuality);
    }

    private static void EnrichFromFantasyPros(dynamic fp, ref string position, ref string team, ref int? bye, ref int? rank)
    {
        rank = fp.RankEcr;
        if (int.TryParse(fp.PlayerByeWeek as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)) bye = parsed;
        if (string.IsNullOrWhiteSpace(position)) position = fp.PlayerPositionId;
        if (string.IsNullOrWhiteSpace(team)) team = fp.PlayerTeamId;
    }

    private static string NormalizeName(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var lower = raw.ToLowerInvariant();
        lower = Regex.Replace(lower, "[^a-z0-9]", string.Empty);
        return lower;
    }
}
