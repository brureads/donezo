using Donezo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Donezo.Api;

public class DonezoDbContextFactory
    : IDesignTimeDbContextFactory<DonezoDbContext>
{
    public DonezoDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<DonezoDbContextFactory>()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string not configured.");

        var options = new DbContextOptionsBuilder<DonezoDbContext>();

        options.UseNpgsql(connectionString);

        return new DonezoDbContext(options.Options);
    }
}