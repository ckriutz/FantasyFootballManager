using System.ComponentModel.DataAnnotations;

namespace FantasyFootballManager.DataService.Models;
public class Team
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
}