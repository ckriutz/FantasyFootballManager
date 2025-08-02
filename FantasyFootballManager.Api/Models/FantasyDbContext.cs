using Microsoft.EntityFrameworkCore;

namespace FantasyFootballManager.DataService.Models;
public class FantasyDbContext : DbContext
{
    public DbSet<SleeperPlayer> SleeperPlayers { get; set; }
    public DbSet<FantasyProsPlayer> FantasyProsPlayers { get; set; }
    public DbSet<SportsDataIoPlayer> SportsDataIoPlayers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<DataStatus> DataStatus { get; set; }
    public DbSet<FantasyActivity> FantasyActivities { get; set; }
    public DbSet<User> Users { get; set; }

    public FantasyDbContext() { }
        
    public FantasyDbContext(DbContextOptions<FantasyDbContext> dbContextOptions) : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Team entity
        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Abbreviation).HasMaxLength(10);
        });

        // Configure SleeperPlayer entity
        modelBuilder.Entity<SleeperPlayer>(entity =>
        {
            entity.Property(e => e.PlayerId).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50); // Increased from 20 to accommodate "Physically Unable to Perform"
            entity.Property(e => e.Position).HasMaxLength(10); // Increased from 5 to accommodate longer position strings
            entity.Property(e => e.SearchFullName).HasMaxLength(200);
            entity.Property(e => e.Weight).HasMaxLength(10);
            entity.Property(e => e.SearchFirstName).HasMaxLength(100);
            entity.Property(e => e.InjuryBodyPart).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.GsisId).HasMaxLength(50);
            entity.Property(e => e.Hashtag).HasMaxLength(100);
            entity.Property(e => e.Height).HasMaxLength(10);
            entity.Property(e => e.DepthChartPosition).HasMaxLength(20); // Increased from 10
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.HighSchool).HasMaxLength(200);
            entity.Property(e => e.College).HasMaxLength(200);
            entity.Property(e => e.InjuryStatus).HasMaxLength(100); // Increased from 50
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.SearchLastName).HasMaxLength(100);
            entity.Property(e => e.InjuryNotes).HasMaxLength(1000); // Increased from 500
            entity.Property(e => e.BirthCountry).HasMaxLength(100);
            entity.Property(e => e.SportRadarId).HasMaxLength(50);
        });

        // Configure FantasyProsPlayer entity
        modelBuilder.Entity<FantasyProsPlayer>(entity =>
        {
            entity.Property(e => e.PlayerName).HasMaxLength(200);
            entity.Property(e => e.SportsdataId).HasMaxLength(50);
            entity.Property(e => e.PlayerTeamId).HasMaxLength(10);
            entity.Property(e => e.PlayerPositionId).HasMaxLength(10); // Increased from 5
            entity.Property(e => e.PlayerPositions).HasMaxLength(50); // Increased from 20
            entity.Property(e => e.PlayerShortName).HasMaxLength(100);
            entity.Property(e => e.PlayerEligibility).HasMaxLength(50);
            entity.Property(e => e.PlayerYahooPositions).HasMaxLength(50); // Increased from 20
            entity.Property(e => e.PlayerPageUrl).HasMaxLength(500);
            entity.Property(e => e.PlayerFilename).HasMaxLength(200);
            entity.Property(e => e.PlayerSquareImageUrl).HasMaxLength(500);
            entity.Property(e => e.PlayerImageUrl).HasMaxLength(500);
            entity.Property(e => e.PlayerYahooId).HasMaxLength(50);
            entity.Property(e => e.CbsPlayerId).HasMaxLength(50);
            entity.Property(e => e.PlayerByeWeek).HasMaxLength(5);
            entity.Property(e => e.RankMin).HasMaxLength(10);
            entity.Property(e => e.RankMax).HasMaxLength(10);
            entity.Property(e => e.RankAve).HasMaxLength(10);
            entity.Property(e => e.RankStd).HasMaxLength(10);
            entity.Property(e => e.PosRank).HasMaxLength(10);
        });

        // Configure SportsDataIoPlayer entity
        modelBuilder.Entity<SportsDataIoPlayer>(entity =>
        {
            entity.Property(e => e.FantasyPlayerKey).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Position).HasMaxLength(10); // Increased from 5
        });

        // Configure DataStatus entity
        modelBuilder.Entity<DataStatus>(entity =>
        {
            entity.Property(e => e.DataSource).HasMaxLength(100);
        });

        // Configure FantasyActivity entity
        modelBuilder.Entity<FantasyActivity>(entity =>
        {
            entity.Property(e => e.User).HasMaxLength(100);
        });

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Auth0Id).HasMaxLength(100);
            entity.Property(e => e.YahooUsername).HasMaxLength(50);
            entity.Property(e => e.YahooLeagueId).HasMaxLength(50);
            entity.Property(e => e.EspnUsername).HasMaxLength(50);
            entity.Property(e => e.EspnLeagueId).HasMaxLength(50);
            entity.Property(e => e.SleeperUsername).HasMaxLength(50);
            entity.Property(e => e.SleeperLeagueId).HasMaxLength(50);
        });
    }
    
}