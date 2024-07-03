using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace FantasyFootballManager.DataService.Models;

public class SleeperPlayer
{
    public int Id { get; set; }

    public string? Status { get; set; }
    
    public DateTime? Birthdate { get; set; }

    public int? Number { get; set; }

    public int? YahooId { get; set; }

    public int? StatsId { get; set; }

    public string? Position { get; set; }

    public string? SearchFullName { get; set; }

    public string? Weight { get; set; }

    public string? SearchFirstName { get; set; }

    public string? InjuryBodyPart { get; set; }

    public int? FantasyDataId { get; set; }

    public int? YearsExp { get; set; }

    public string? LastName { get; set; }

    public int? RotoworldId { get; set; }

    public string? GsisId { get; set; }

    public long? NewsUpdated { get; set; }

    public int? EspnId { get; set; }

    public string? Hashtag { get; set; }

    public int? Age { get; set; }

    public string? Height { get; set; }

    public Team? Team { get; set; }

    public string? DepthChartPosition { get; set; }

    public int? SearchRank { get; set; }

    public string? FullName { get; set; }

    public string? HighSchool { get; set; }

    public int? DepthChartOrder { get; set; }

    public int? RotowireId { get; set; }

    public string PlayerId { get; set; }

    public string? College { get; set; }

    public string? InjuryStatus { get; set; }

    public string? FirstName { get; set; }

    public bool IsActive { get; set; } = false;

    public int? SwishId { get; set; }

    public string? SearchLastName { get; set; }

    public string? InjuryNotes { get; set; }

    public string? BirthCountry { get; set; }

    public string? SportRadarId { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.Now;
}