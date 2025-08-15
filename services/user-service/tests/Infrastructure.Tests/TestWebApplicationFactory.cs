using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using UserService.Application.Ports;
using UserService.Infrastructure.Repositories;

namespace UserService.Tests.Infrastructure.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use Development to enable Swagger for API documentation tests
        builder.UseEnvironment("Development");
        
        // Configure test-specific settings to disable auto-migration and PostgreSQL health check
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("MIGRATE_AT_STARTUP", "false"),
                new KeyValuePair<string, string?>("ConnectionStrings:PostgreSQL", "Server=localhost;Port=9999;Database=test_db_does_not_exist;User Id=test;Password=test;")
            });
        });
        
        builder.ConfigureServices(services =>
        {
            // Configure JSON serialization to avoid PipeWriter issues in tests
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            
            // Override EF Core PostgreSQL registration with in-memory for testing
            // Remove the EF Core registration
            var efDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(UserService.Infrastructure.Persistence.FinmanDbContext));
            if (efDescriptor != null)
            {
                services.Remove(efDescriptor);
            }
            
            // Replace EfUserRepository with InMemoryUserRepository for tests
            var repoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserRepository));
            if (repoDescriptor != null)
            {
                services.Remove(repoDescriptor);
            }
            
            services.AddScoped<IUserRepository, InMemoryUserRepository>();

            // Override health checks by clearing all and adding a simple healthy check
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Clear();
            });
            
            // Add a test health check with the same name as the main application expects
            services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy("Service is running"));
        });
    }
}