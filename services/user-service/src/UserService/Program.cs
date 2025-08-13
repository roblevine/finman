using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

// Add Problem Details support with proper configuration
builder.Services.AddProblemDetails(options =>
{
    // Set default problem details options
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = context.HttpContext.Request.Path;
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Finman User Service API",
        Version = "v1",
        Description = "User registration and management API for the Finman application",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Finman Development Team"
        }
    });
    
    // Enable XML comments for better API documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add health checks (we'll add Npgsql later conditionally)
var healthChecksBuilder = builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Service is running"));

// Persistence configuration
var connectionString = builder.Configuration.GetValue<string>("POSTGRES_CONNECTION")
                      ?? builder.Configuration.GetConnectionString("Postgres")
                      ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION");

if (!string.IsNullOrWhiteSpace(connectionString))
{
    // EF Core with Npgsql
    builder.Services.AddDbContext<UserService.Infrastructure.Persistence.FinmanDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Use EF repository
    builder.Services.AddScoped<UserService.Application.Ports.IUserRepository, UserService.Infrastructure.Repositories.EfUserRepository>();
}
else
{
    // Fallback to in-memory repository for non-DB scenarios
    builder.Services.AddScoped<UserService.Application.Ports.IUserRepository, UserService.Infrastructure.Repositories.InMemoryUserRepository>();
}
builder.Services.AddScoped<UserService.Application.Ports.IPasswordHasher, UserService.Infrastructure.Security.BCryptPasswordHasher>();
builder.Services.AddScoped<UserService.Application.UseCases.RegisterUserHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Auto-migration hook (Development only)
var migrateAtStartup = app.Configuration.GetValue<bool>("MIGRATE_AT_STARTUP")
                      || app.Configuration.GetValue<bool>("MigrateAtStartup");
if (app.Environment.IsDevelopment() && migrateAtStartup && !string.IsNullOrWhiteSpace(connectionString))
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserService.Infrastructure.Persistence.FinmanDbContext>();
        db.Database.Migrate();
        app.Logger.LogInformation("EF Core migrations applied at startup.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to apply EF Core migrations at startup.");
    }
}

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment() && !app.Environment.EnvironmentName.Equals("Test", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Make Program class public for testing
public partial class Program { }