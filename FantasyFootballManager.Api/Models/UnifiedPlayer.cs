using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FantasyFootballManager.DataService.Models;

/// <summary>
/// Unified minimal player projection combining Sleeper (canonical id), FantasyPros (rank/bye), and SportsDataIO (projections/ADP).
/// This DTO keeps only fields needed for AI draft analysis.
/// </summary>
public sealed record UnifiedPlayer
{
    public string PlayerId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public int? DepthChartOrder { get; init; }
    public string TeamAbbreviation { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public int? ByeWeek { get; init; }
    public int? RankEcr { get; init; }
    public double? ProjectedFantasyPoints { get; init; }
    public double? AverageDraftPosition { get; init; }
    public double? AverageDraftPositionPpr { get; init; }
    public double? AuctionValue { get; init; }
    public double? PlayerOwnedAvg { get; init; }
    public string? PlayerImageUrl { get; init; }
    public string? PosRank { get; init; }
    public int? Tier { get; init; }
    public int Age { get; init; }
    public string? Weight { get; init; }
    public string Height { get; init; } = string.Empty; // Height in "6'2"" format, can be parsed later
    public string College { get; init; } = string.Empty;
    public int YearsExp { get; init; }
    public string Status { get; init; } = string.Empty; // e.g., "Active", "Injured", etc.  
    public string MatchQuality { get; init; } = string.Empty; // "ExactId", "ExactName", "NameFallback", "SleeperOnly"
    public DateTime? SleeperLastUpdated { get; init; }
    public DateTime? FantasyProsLastUpdated { get; init; }
    public DateTime? SportsDataIoLastUpdated { get; init; }
    public bool IsThumbsUp { get; init; }
    public bool IsThumbsDown { get; init; }
    public bool IsDraftedOnMyTeam { get; init; }
    public bool IsDraftedOnOtherTeam { get; init; }

    // Default constructor
    public UnifiedPlayer() { }
}

public interface IUnifiedPlayerService
{
    Task<IReadOnlyList<UnifiedPlayer>> GetUnifiedPlayersAsync(int? topNRanked = null, string? userId = null, CancellationToken ct = default);
    Task<UnifiedPlayer?> GetUnifiedPlayerAsync(string sleeperPlayerId, string? userId = null, CancellationToken ct = default);
}

public sealed class UnifiedPlayerService : IUnifiedPlayerService
{
    private readonly FantasyDbContext _db;

    public UnifiedPlayerService(FantasyDbContext db) => _db = db;

    public async Task<IReadOnlyList<UnifiedPlayer>> GetUnifiedPlayersAsync(int? topNRanked = null, string? userId = null, CancellationToken ct = default)
    {
        var sleeper = await _db.SleeperPlayers.AsNoTracking()
            .Where(p => p.SearchRank != 9999999)
            .Select(p => new { p.PlayerId, p.FullName, p.Position, p.DepthChartOrder, p.SportRadarId, TeamAbbreviation = p.Team != null ? p.Team.Abbreviation : null, TeamName = p.Team != null ? p.Team.Name : null, p.Age, p.Weight, p.College, p.Height, p.YearsExp, p.Status, p.LastUpdated })
            .ToListAsync(ct);
        var fantasy = await _db.FantasyProsPlayers.AsNoTracking()
            .Select(p => new { p.SportsdataId, p.PlayerName, p.PlayerPositionId, p.PlayerTeamId, p.PlayerByeWeek, p.RankEcr, p.PlayerOwnedAvg, p.PlayerImageUrl, p.PosRank, p.Tier, p.LastUpdated })
            .ToListAsync(ct);
        var sports = await _db.SportsDataIoPlayers.AsNoTracking()
            .Select(p => new { p.Name, p.Position, p.ByeWeek, p.ProjectedFantasyPoints, p.AverageDraftPosition, p.AverageDraftPositionPPR, p.AuctionValue, Team = p.PlayerTeam != null ? p.PlayerTeam.Abbreviation : null, p.LastUpdated })
            .ToListAsync(ct);

        // Fetch user activity data if userId is provided
        var userActivities = new Dictionary<string, (bool IsThumbsUp, bool IsThumbsDown, bool IsDraftedOnMyTeam, bool IsDraftedOnOtherTeam)>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var activities = await _db.FantasyActivities.AsNoTracking()
                .Where(a => a.User == userId)
                .Select(a => new { PlayerId = a.PlayerId.ToString(), a.IsThumbsUp, a.IsThumbsDown, a.IsDraftedOnMyTeam, a.IsDraftedOnOtherTeam })
                .ToListAsync(ct);
            
            userActivities = activities.ToDictionary(
                a => a.PlayerId, 
                a => (a.IsThumbsUp, a.IsThumbsDown, a.IsDraftedOnMyTeam, a.IsDraftedOnOtherTeam)
            );
        }

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

        var unified = new List<UnifiedPlayer>(sleeper.Count);

        foreach (var sp in sleeper)
        {
            var name = sp.FullName ?? string.Empty;
            var position = sp.Position ?? string.Empty;
            int? depthChartOrder = sp.DepthChartOrder;
            var teamName = sp.TeamName ?? string.Empty;
            var teamAbbreviation = sp.TeamAbbreviation ?? string.Empty;
            int? bye = null;
            int? rank = null;
            double? proj = null;
            double? adp = null;
            double? adpPpr = null;
            double? auctionValue = null;
            int age = sp.Age ?? 0; // Default age, can be updated later if available
            string weight = sp.Weight?.ToString() ?? string.Empty; // Default weight, can be updated later if available
            string height = sp.Height ?? string.Empty; // Default height, can be updated later if available
            int yearsExp = sp.YearsExp ?? 0; // Default years of experience, can be updated later if available
            string college = sp.College ?? string.Empty; // Default college, can be updated later if available
            string status = sp.Status ?? string.Empty; // Default status, can be updated later if available
            var matchQuality = "SleeperOnly";
            string? playerImageUrl = null;
            double? playerOwnedAvg = null;
            string? posRank = null;
            int? tier = null;
            DateTime? sleeperLastUpdated = sp.LastUpdated;
            DateTime? fantasyProsLastUpdated = null;
            DateTime? sportsDataIoLastUpdated = null;

            if (!string.IsNullOrWhiteSpace(sp.SportRadarId) && fantasyBySportsDataId.TryGetValue(sp.SportRadarId, out var fpById))
            {
                EnrichFromFantasyPros(fpById, ref position, ref teamName, ref teamAbbreviation, ref bye, ref rank, ref playerImageUrl, ref playerOwnedAvg, ref posRank, ref tier, ref fantasyProsLastUpdated);
                matchQuality = "ExactId";
            }
            else
            {
                var nameKey = NormalizeName(name);
                if (fantasyByName.TryGetValue(nameKey, out var fpByName))
                {
                    EnrichFromFantasyPros(fpByName, ref position, ref teamName, ref teamAbbreviation, ref bye, ref rank, ref playerImageUrl, ref playerOwnedAvg, ref posRank, ref tier, ref fantasyProsLastUpdated);
                    matchQuality = "ExactName";
                }
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var key = NormalizeName(name);
                if (sportsByName.TryGetValue(key, out var sd))
                {
                    proj = sd.ProjectedFantasyPoints;
                    sportsDataIoLastUpdated = sd.LastUpdated;
                    if (sd.ByeWeek.HasValue && bye == null) bye = sd.ByeWeek;
                    if (string.IsNullOrWhiteSpace(position)) position = sd.Position;
                    if (string.IsNullOrWhiteSpace(teamName)) teamName = sd.Team ?? teamName;
                    if (sd.AverageDraftPosition.HasValue) adp = sd.AverageDraftPosition;
                    if (sd.AverageDraftPositionPPR.HasValue) adpPpr = sd.AverageDraftPositionPPR;
                    if (sd.AuctionValue.HasValue) auctionValue = sd.AuctionValue;
                    if (matchQuality == "SleeperOnly") matchQuality = "NameFallback";
                }
            }

            if (string.IsNullOrWhiteSpace(name)) name = sp.PlayerId;
            if (string.IsNullOrWhiteSpace(position)) position = "?";

            // Get user activity data for this player
            bool isThumbsUp = false;
            bool isThumbsDown = false;
            bool isDraftedOnMyTeam = false;
            bool isDraftedOnOtherTeam = false;

            if (userActivities.TryGetValue(sp.PlayerId, out var activity))
            {
                isThumbsUp = activity.IsThumbsUp;
                isThumbsDown = activity.IsThumbsDown;
                isDraftedOnMyTeam = activity.IsDraftedOnMyTeam;
                isDraftedOnOtherTeam = activity.IsDraftedOnOtherTeam;

            }

            unified.Add(new UnifiedPlayer
            {
                PlayerId = sp.PlayerId,
                Name = name,
                Position = position,
                DepthChartOrder = depthChartOrder,
                TeamAbbreviation = teamAbbreviation,
                TeamName = teamName,
                ByeWeek = bye,
                RankEcr = rank,
                ProjectedFantasyPoints = proj,
                AverageDraftPosition = adp,
                AverageDraftPositionPpr = adpPpr,
                AuctionValue = auctionValue,
                Age = age,
                Weight = weight,
                Height = height,
                YearsExp = yearsExp,
                College = college,
                Status = status,
                PlayerOwnedAvg = playerOwnedAvg,
                PlayerImageUrl = playerImageUrl,
                PosRank = posRank,
                Tier = tier,
                MatchQuality = matchQuality,
                SleeperLastUpdated = sleeperLastUpdated,
                FantasyProsLastUpdated = fantasyProsLastUpdated,
                SportsDataIoLastUpdated = sportsDataIoLastUpdated,
                IsThumbsUp = isThumbsUp,
                IsThumbsDown = isThumbsDown,
                IsDraftedOnMyTeam = isDraftedOnMyTeam,
                IsDraftedOnOtherTeam = isDraftedOnOtherTeam
            });
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

    public async Task<UnifiedPlayer?> GetUnifiedPlayerAsync(string sleeperPlayerId, string? userId = null, CancellationToken ct = default)
    {
        var basePlayer = await _db.SleeperPlayers.AsNoTracking()
            .Where(p => p.PlayerId == sleeperPlayerId && p.SearchRank != 9999999)
            .Select(p => new { p.PlayerId, p.FullName, p.Position, p.DepthChartOrder, p.SportRadarId, TeamAbbreviation = p.Team != null ? p.Team.Abbreviation : null, TeamName = p.Team != null ? p.Team.Name : null, p.Age, p.Weight, p.College, p.Height, p.YearsExp, p.Status, p.LastUpdated })
            .FirstOrDefaultAsync(ct);
        if (basePlayer == null) return null;

        int? bye = null;
        int? rank = null;
        double? proj = null;
        double? adp = null;
        double? adpPpr = null;
        double? auctionValue = null;
        double? playerOwnedAvg = null;
        string? posRank = null;
        int? tier = null;
        int age = basePlayer.Age ?? 0; // Default age, can be updated later if available
        string weight = basePlayer.Weight ?? string.Empty; // Default weight, can be updated later if available
        string college = basePlayer.College ?? string.Empty; // Default college, can be updated later if available
        string matchQuality = "SleeperOnly";
        string position = basePlayer.Position ?? string.Empty;
        string teamName = basePlayer.TeamName ?? string.Empty;
        string teamAbbreviation = basePlayer.TeamAbbreviation ?? string.Empty;
        string name = basePlayer.FullName ?? string.Empty;
        string height = basePlayer.Height ?? string.Empty;
        int yearsExp = basePlayer.YearsExp ?? 0; // Default years of experience, can be updated later if available
        string status = basePlayer.Status ?? string.Empty; // Default status, can be updated later if available
        int? depthChartOrder = basePlayer.DepthChartOrder;
        string? playerImageUrl = null;
        DateTime? sleeperLastUpdated = basePlayer.LastUpdated;
        DateTime? fantasyProsLastUpdated = null;
        DateTime? sportsDataIoLastUpdated = null;

        // Fetch user activity data if userId is provided
        bool isThumbsUp = false;
        bool isThumbsDown = false;
        bool isDraftedOnMyTeam = false;
        bool isDraftedOnOtherTeam = false;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var activity = await _db.FantasyActivities.AsNoTracking()
                .Where(a => a.User == userId && a.PlayerId.ToString() == sleeperPlayerId)
                .Select(a => new { a.IsThumbsUp, a.IsThumbsDown, a.IsDraftedOnMyTeam, a.IsDraftedOnOtherTeam })
                .FirstOrDefaultAsync(ct);

            if (activity != null)
            {
                isThumbsUp = activity.IsThumbsUp;
                isThumbsDown = activity.IsThumbsDown;
                isDraftedOnMyTeam = activity.IsDraftedOnMyTeam;
                isDraftedOnOtherTeam = activity.IsDraftedOnOtherTeam;
            }
        }

        if (!string.IsNullOrWhiteSpace(basePlayer.SportRadarId))
        {
            var fp = await _db.FantasyProsPlayers.AsNoTracking()
                .Where(f => f.SportsdataId == basePlayer.SportRadarId)
                .OrderBy(f => f.RankEcr)
                .FirstOrDefaultAsync(ct);
            if (fp != null)
            {
                EnrichFromFantasyPros(fp, ref position, ref teamName, ref teamAbbreviation, ref bye, ref rank, ref playerImageUrl, ref playerOwnedAvg, ref posRank, ref tier, ref fantasyProsLastUpdated);
                matchQuality = "ExactId";
            }
        }
        if (matchQuality == "SleeperOnly")
        {
            // Try exact name match first, then fallback to client evaluation if needed
            var fp = await _db.FantasyProsPlayers.AsNoTracking()
                .Where(f => f.PlayerName == name)
                .OrderBy(f => f.RankEcr)
                .FirstOrDefaultAsync(ct);
                
            // If no exact match, use client evaluation on a smaller subset
            if (fp == null && !string.IsNullOrWhiteSpace(name))
            {
                var key = NormalizeName(name);
                var nameParts = name.Split(' ');
                var firstName = nameParts[0];
                var lastName = nameParts[nameParts.Length - 1];
                
                var fps = await _db.FantasyProsPlayers.AsNoTracking()
                    .Where(f => f.PlayerName.Contains(firstName) || f.PlayerName.Contains(lastName))
                    .ToListAsync(ct);
                fp = fps.Where(f => NormalizeName(f.PlayerName) == key)
                    .OrderBy(f => f.RankEcr)
                    .FirstOrDefault();
            }
            if (fp != null)
            {
                EnrichFromFantasyPros(fp, ref position, ref teamName, ref teamAbbreviation, ref bye, ref rank, ref playerImageUrl, ref playerOwnedAvg, ref posRank, ref tier, ref fantasyProsLastUpdated);
                matchQuality = "ExactName";
            }
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            // Try exact name match first, then fallback to client evaluation if needed
            var sd = await _db.SportsDataIoPlayers.AsNoTracking()
                .Where(s => s.Name == name)
                .FirstOrDefaultAsync(ct);
                
            // If no exact match, use client evaluation on a smaller subset
            if (sd == null)
            {
                var key = NormalizeName(name);
                var nameParts = name.Split(' ');
                var firstName = nameParts[0];
                var lastName = nameParts[nameParts.Length - 1];
                
                var sds = await _db.SportsDataIoPlayers.AsNoTracking()
                    .Where(s => s.Name.Contains(firstName) || s.Name.Contains(lastName))
                    .ToListAsync(ct);
                sd = sds.FirstOrDefault(s => NormalizeName(s.Name) == key);
            }
            
            if (sd != null)
            {
                proj = sd.ProjectedFantasyPoints;
                sportsDataIoLastUpdated = sd.LastUpdated;
                if (sd.ByeWeek.HasValue && bye == null) bye = sd.ByeWeek;
                if (string.IsNullOrWhiteSpace(position)) position = sd.Position;
                if (string.IsNullOrWhiteSpace(teamName) && sd.PlayerTeam != null) teamName = sd.PlayerTeam.Name;
                if (string.IsNullOrWhiteSpace(teamAbbreviation) && sd.PlayerTeam != null) teamAbbreviation = sd.PlayerTeam.Abbreviation;
                if (sd.AuctionValue.HasValue) auctionValue = sd.AuctionValue;
                if (sd.AverageDraftPosition.HasValue) adp = sd.AverageDraftPosition;
                if (sd.AverageDraftPositionPPR.HasValue) adpPpr = sd.AverageDraftPositionPPR;
                if (matchQuality == "SleeperOnly") matchQuality = "NameFallback";
            }
        }

        if (string.IsNullOrWhiteSpace(name)) name = sleeperPlayerId;
        if (string.IsNullOrWhiteSpace(position)) position = "?";

        return new UnifiedPlayer
        {
            PlayerId = sleeperPlayerId,
            Name = name,
            Position = position,
            DepthChartOrder = depthChartOrder,
            TeamAbbreviation = teamAbbreviation,
            TeamName = teamName,
            ByeWeek = bye,
            RankEcr = rank,
            ProjectedFantasyPoints = proj,
            PlayerOwnedAvg = playerOwnedAvg,
            AverageDraftPosition = adp,
            AverageDraftPositionPpr = adpPpr,
            AuctionValue = auctionValue,
            MatchQuality = matchQuality,
            Age = age,
            Status = status,
            Weight = weight,
            College = college,
            Height = height,
            YearsExp = yearsExp,
            SleeperLastUpdated = sleeperLastUpdated,
            FantasyProsLastUpdated = fantasyProsLastUpdated,
            SportsDataIoLastUpdated = sportsDataIoLastUpdated,
            PlayerImageUrl = playerImageUrl,
            PosRank = posRank,
            Tier = tier,
            IsThumbsUp = isThumbsUp,
            IsThumbsDown = isThumbsDown,
            IsDraftedOnMyTeam = isDraftedOnMyTeam,
            IsDraftedOnOtherTeam = isDraftedOnOtherTeam
        };
    }

    private static void EnrichFromFantasyPros(dynamic fp, ref string position, ref string teamName, ref string teamAbbreviation, ref int? bye, ref int? rank, ref string? playerImageUrl, ref double? playerOwnedAvg, ref string? posRank, ref int? tier, ref DateTime? fantasyProsLastUpdated)
    {
        rank = fp.RankEcr;
        fantasyProsLastUpdated = fp.LastUpdated;
        playerImageUrl = fp.PlayerImageUrl;
        playerOwnedAvg = fp.PlayerOwnedAvg;
        posRank = fp.PosRank;
        tier = fp.Tier;
        if (int.TryParse(fp.PlayerByeWeek as string, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)) bye = parsed;
        if (string.IsNullOrWhiteSpace(position)) position = fp.PlayerPositionId;
        if (string.IsNullOrWhiteSpace(teamName)) teamName = fp.PlayerTeamId;
        if (string.IsNullOrWhiteSpace(teamAbbreviation)) teamAbbreviation = fp.PlayerTeamId; // Use PlayerTeamId for abbreviation as well
    }

    private static string NormalizeName(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var lower = raw.ToLowerInvariant();
        lower = Regex.Replace(lower, "[^a-z0-9]", string.Empty);
        return lower;
    }
}
