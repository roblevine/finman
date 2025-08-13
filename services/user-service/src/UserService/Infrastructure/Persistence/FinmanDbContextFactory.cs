using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserService.Infrastructure.Persistence;

public class FinmanDbContextFactory : IDesignTimeDbContextFactory<FinmanDbContext>
{
    public FinmanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinmanDbContext>();

        // Allow override via env var; fallback to a sensible local default
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
                               ?? "Host=localhost;Port=5432;Database=finman;Username=finman;Password=finman";

        optionsBuilder.UseNpgsql(connectionString);
        return new FinmanDbContext(optionsBuilder.Options);
    }
}
