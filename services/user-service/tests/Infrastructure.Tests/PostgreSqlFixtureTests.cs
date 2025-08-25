using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Persistence;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

public class PostgreSqlFixtureTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _fixture;

    public PostgreSqlFixtureTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void PostgreSqlContainer_ShouldStart_AndExposeConnectionString()
    {
        // Arrange & Act
        var connectionString = _fixture.ConnectionString;

        // Assert
        Assert.NotNull(connectionString);
        Assert.NotEmpty(connectionString);
        Assert.Contains("finman_test", connectionString);
    }

    [Fact]
    public async Task PostgreSqlContainer_ShouldHave_MigrationsApplied()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<FinmanDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        // Act
        await using var context = new FinmanDbContext(options);
        var canConnect = await context.Database.CanConnectAsync();
        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();

        // Assert
        Assert.True(canConnect);
        Assert.NotEmpty(appliedMigrations);
        Assert.Contains(appliedMigrations, m => m.Contains("InitialUsers"));
    }

    [Fact]
    public async Task PostgreSqlContainer_ShouldHave_UsersTableWithCorrectSchema()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<FinmanDbContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;

        // Act
        await using var context = new FinmanDbContext(options);
        
        // Check if we can query the users table structure using DbSet query
        var usersDbSet = context.Users;
        Assert.NotNull(usersDbSet);
        
        // Simple test - try to query (this will validate table exists)
        var userCount = await context.Users.CountAsync();

        // Assert
        Assert.True(userCount >= 0, "Should be able to query users table");
    }
}