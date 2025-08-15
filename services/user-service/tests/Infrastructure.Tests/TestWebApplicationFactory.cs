using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace UserService.Tests.Infrastructure.Tests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureServices(services =>
        {
            // Configure JSON serialization to avoid PipeWriter issues in tests
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        });
    }
}