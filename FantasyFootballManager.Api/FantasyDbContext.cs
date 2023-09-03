using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Design;

public class FantasyDbContext : DbContext
{
    public DbSet<Team> Teams { get; set; }
    public DbSet<DataStatus> DataStatus { get; set; }

    //public FantasyDbContext() { }
        
    public FantasyDbContext(DbContextOptions<FantasyDbContext> dbContextOptions) : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
}