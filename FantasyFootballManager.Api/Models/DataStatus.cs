
using System;
using System.ComponentModel.DataAnnotations;

namespace FantasyFootballManager.DataService.Models;
public class DataStatus
{
        [Key]
        public int Id { get; set; }
        public string DataSource { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}