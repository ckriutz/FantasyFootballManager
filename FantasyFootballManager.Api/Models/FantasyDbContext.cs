using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Design;

namespace FantasyFootballManager.DataService.Models;
public class FantasyDbContext : DbContext
{
    //public DbSet<SleeperPlayer> SleeperPlayers { get; set; }
    //public DbSet<FantasyProsPlayer> FantasyProsPlayers { get; set; }
    //public DbSet<SportsDataIoPlayer> SportsDataIoPlayers { get; set; }
    //public DbSet<Team> Teams { get; set; }
    public DbSet<DataStatus> DataStatus { get; set; }

    //public FantasyDbContext() { }
        
    public FantasyDbContext(DbContextOptions<FantasyDbContext> dbContextOptions) : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
}