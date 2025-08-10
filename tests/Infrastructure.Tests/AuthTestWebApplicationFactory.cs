using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using UserService.Application.Ports;
using UserService.Infrastructure.Repositories;

namespace UserService.Tests.Infrastructure.Tests;

/// <summary>
/// Specialized WebApplicationFactory for AuthController integration tests.
/// Uses singleton UserRepository to ensure data persistence across HTTP requests within the same test.
/// </summary>
public class AuthTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        
        builder.ConfigureServices(services =>
        {
            // Configure JSON serialization to avoid PipeWriter issues in tests
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            
            // Replace scoped repository with singleton for auth integration tests
            // This ensures the same repository instance is used across multiple HTTP requests in tests
            // allowing duplicate email/username detection to work correctly
            var serviceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IUserRepository));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        });
    }
}
