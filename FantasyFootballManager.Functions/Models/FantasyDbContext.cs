using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.Functions.Models
{
    public class FantasyContext : DbContext
    {
        public DbSet<FootballPlayer> FootballPlayers { get; set; }
        public DbSet<ImportStatus> ImportStatuses { get; set; }

        public FantasyContext() { }
        
        public FantasyContext(DbContextOptions<FantasyContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var postgresConnectionString = System.Environment.GetEnvironmentVariable("PostgresConnectionString");
            optionsBuilder.UseNpgsql(postgresConnectionString);
        }
    }
}