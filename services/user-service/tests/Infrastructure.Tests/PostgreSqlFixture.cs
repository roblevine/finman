using Microsoft.EntityFrameworkCore;
using Npgsql;
using UserService.Infrastructure.Persistence;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly string _testDatabaseName;
    private readonly string _baseConnectionString;

    public PostgreSqlFixture()
    {
        // Generate unique database name for this test run
        _testDatabaseName = $"finman_test_{Guid.NewGuid():N}";
        
        // Use the existing PostgreSQL container connection
        // This connects via Docker host since PostgreSQL port 5432 is exposed to host
        _baseConnectionString = "Host=host.docker.internal;Port=5432;Username=finman;Password=finman_dev_password;Database=postgres";
    }

    public string ConnectionString => _baseConnectionString.Replace("Database=postgres", $"Database={_testDatabaseName}");

    public async Task InitializeAsync()
    {
        await CreateTestDatabaseAsync();
        await ApplyMigrationsAsync();
    }

    public async Task DisposeAsync()
    {
        await DropTestDatabaseAsync();
    }

    private async Task CreateTestDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_baseConnectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand($"CREATE DATABASE \"{_testDatabaseName}\"", connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task DropTestDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_baseConnectionString);
        await connection.OpenAsync();
        
        // Terminate connections to the test database before dropping
        await using var terminateCommand = new NpgsqlCommand(
            $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{_testDatabaseName}' AND pid <> pg_backend_pid()", 
            connection);
        await terminateCommand.ExecuteNonQueryAsync();
        
        await using var dropCommand = new NpgsqlCommand($"DROP DATABASE IF EXISTS \"{_testDatabaseName}\"", connection);
        await dropCommand.ExecuteNonQueryAsync();
    }

    private async Task ApplyMigrationsAsync()
    {
        var options = new DbContextOptionsBuilder<FinmanDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new FinmanDbContext(options);
        await context.Database.MigrateAsync();
    }
}