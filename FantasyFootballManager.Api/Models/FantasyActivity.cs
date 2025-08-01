namespace FantasyFootballManager.DataService.Models;

public class FantasyActivity
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public bool IsThumbsUp { get; set; } = false;
    public bool IsThumbsDown { get; set; } = false;
    public bool IsDraftedOnMyTeam { get; set; } = false;
    public bool IsDraftedOnOtherTeam { get; set; } = false;
}