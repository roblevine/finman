using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserService.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for FinmanDbContext to support EF Core migrations.
/// This factory is used when running dotnet ef commands during development.
/// </summary>
public class FinmanDbContextFactory : IDesignTimeDbContextFactory<FinmanDbContext>
{
    public FinmanDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinmanDbContext>();
        
        // Use a default connection string for migrations
        // This is only used during design-time, not runtime
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=finman_migrations;Username=finman;Password=finman");
        
        return new FinmanDbContext(optionsBuilder.Options);
    }
}
