using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FantasyFootballManager.DataService.Models;

public class FantasyDbContextFactory : IDesignTimeDbContextFactory<FantasyDbContext>
{
    public FantasyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FantasyDbContext>();

        // Use your connection string here, or read from environment variable
        var connectionString = System.Environment.GetEnvironmentVariable("postgresConnectionString");

        optionsBuilder.UseNpgsql(connectionString);

        return new FantasyDbContext(optionsBuilder.Options);
    }
}