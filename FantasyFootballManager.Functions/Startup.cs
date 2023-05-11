using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FantasyFootballManager.Functions.Startup))]
namespace FantasyFootballManager.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var postgresConnectionString = System.Environment.GetEnvironmentVariable("PostgresConnectionString");
            builder.Services.AddDbContext<FantasyFootballManager.Functions.Models.FantasyContext>(x => x.UseNpgsql(postgresConnectionString));
        }
    }
}