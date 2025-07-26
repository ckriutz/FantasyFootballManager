namespace FantasyFootballManager.DataService.Models;

public class FantasyActivity
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public int PlayerId { get; set; }
    public Boolean IsThumbsUp { get; set; } = false;
    public Boolean IsThumbsDown { get; set; } = false;
    public Boolean IsDraftedOnMyTeam { get; set; } = false;
    public Boolean IsDraftedOnOtherTeam { get; set; } = false;
}