using System.ComponentModel.DataAnnotations;

namespace FantasyFootballManager.DataService.Models;
public class Team
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
}