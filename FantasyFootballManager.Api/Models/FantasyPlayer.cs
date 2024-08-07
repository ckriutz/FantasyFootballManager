using System;

namespace FantasyFootballManager.DataService.Models;

public class FantasyPlayer
{
    public FantasyPlayer() { }

    public int Id { get; set; }
    public string PlayerId { get; set; }

    // These are the operational items for the FantasyPlayer
    public bool IsThumbsUp { get; set; } = false;
    public bool IsThumbsDown { get; set; } = false;
    public bool IsTaken { get; set; } = false;
    public bool IsOnMyTeam { get; set; } = false;

    // These are draft result information from Sleeper
    public string PickedBy { get; set; } = string.Empty;
    public int? PickNumber { get; set; }
    public int? PickRound { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.Now;
}