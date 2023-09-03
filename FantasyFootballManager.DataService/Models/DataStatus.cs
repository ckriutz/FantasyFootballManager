using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace FantasyFootballManager.DataService.Models;
public class DataStatus
{
        public int Id { get; set; }
        public string DataSource { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
}