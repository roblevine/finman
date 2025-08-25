using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using UserService.Application.Ports;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Persistence.Repositories;

namespace UserService.Tests.Infrastructure.Tests;

public class TestcontainersWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlFixture _fixture;

    public TestcontainersWebApplicationFactory(PostgreSqlFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use Development to enable Swagger for API documentation tests
        builder.UseEnvironment("Development");
        
        // Configure test-specific settings with PostgreSQL connection
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("MIGRATE_AT_STARTUP", "false"), // Migrations handled by fixture
                new KeyValuePair<string, string?>("ConnectionStrings:PostgreSQL", _fixture.ConnectionString)
            });
        });
        
        builder.ConfigureServices(services =>
        {
            // Configure JSON serialization to avoid PipeWriter issues in tests
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            
            // Remove existing EF Core registration
            var efDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(FinmanDbContext));
            if (efDescriptor != null)
            {
                services.Remove(efDescriptor);
            }
            
            // Remove existing repository registration
            var repoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserRepository));
            if (repoDescriptor != null)
            {
                services.Remove(repoDescriptor);
            }
            
            // Register PostgreSQL DbContext with test database connection
            services.AddDbContext<FinmanDbContext>(options =>
            {
                options.UseNpgsql(_fixture.ConnectionString);
            });
            
            // Register EfUserRepository for PostgreSQL testing
            services.AddScoped<IUserRepository, EfUserRepository>();

            // Override health checks - disable PostgreSQL health check for tests
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Clear();
            });
            
            // Add a test health check with the same name as the main application expects
            services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy("Service is running"));
        });
    }
}