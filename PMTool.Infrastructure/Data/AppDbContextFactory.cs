using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PMTool.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Get the path to the Web project to find appsettings.json
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "PMTool.Web");
        
        // If that fails (e.g. running from root or other project), try current directory
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Directory.GetCurrentDirectory();
        }

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=OUSL_PMDB;Trusted_Connection=True;MultipleActiveResultSets=true";

        builder.UseSqlServer(connectionString);

        return new AppDbContext(builder.Options);
    }
}
