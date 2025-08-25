using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

/// <summary>
/// Base class for tests that can run with either InMemory or PostgreSQL backends
/// controlled by the USE_TESTCONTAINERS environment variable.
/// 
/// Usage:
/// - Default (USE_TESTCONTAINERS not set or false): Uses InMemoryUserRepository for fast feedback
/// - USE_TESTCONTAINERS=true: Uses PostgreSQL with TestcontainersWebApplicationFactory for realistic integration
/// </summary>
public abstract class SwitchableWebApplicationFactoryTests : IClassFixture<PostgreSqlFixture>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    protected readonly bool IsUsingPostgreSQL;

    protected SwitchableWebApplicationFactoryTests(PostgreSqlFixture? fixture = null)
    {
        var useTestcontainers = Environment.GetEnvironmentVariable("USE_TESTCONTAINERS");
        IsUsingPostgreSQL = string.Equals(useTestcontainers, "true", StringComparison.OrdinalIgnoreCase);

        if (IsUsingPostgreSQL)
        {
            if (fixture == null)
                throw new InvalidOperationException("PostgreSqlFixture is required when USE_TESTCONTAINERS=true");
                
            _factory = new TestcontainersWebApplicationFactory(fixture);
        }
        else
        {
            _factory = new TestWebApplicationFactory();
        }

        _client = _factory.CreateClient();
    }

    protected HttpClient Client => _client;

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
        GC.SuppressFinalize(this);
    }
}