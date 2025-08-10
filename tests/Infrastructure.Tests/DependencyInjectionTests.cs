using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Ports;
using UserService.Application.UseCases;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Security;
using Xunit;

namespace UserService.Tests.Infrastructure.Tests;

/// <summary>
/// Tests for dependency injection configuration in Program.cs
/// </summary>
public class DependencyInjectionTests : IDisposable
{
    private readonly TestWebApplicationFactory _factory;
    private readonly IServiceScope _scope;
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionTests()
    {
        _factory = new TestWebApplicationFactory();
        _scope = _factory.Services.CreateScope();
        _serviceProvider = _scope.ServiceProvider;
    }

    [Fact]
    public void ServiceProvider_ShouldResolveIUserRepository()
    {
        // Act
        var repository = _serviceProvider.GetService<IUserRepository>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<InMemoryUserRepository>(repository);
    }

    [Fact]
    public void ServiceProvider_ShouldResolveIPasswordHasher()
    {
        // Act
        var hasher = _serviceProvider.GetService<IPasswordHasher>();

        // Assert
        Assert.NotNull(hasher);
        Assert.IsType<BCryptPasswordHasher>(hasher);
    }

    [Fact]
    public void ServiceProvider_ShouldResolveRegisterUserHandler()
    {
        // Act
        var handler = _serviceProvider.GetService<RegisterUserHandler>();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public void ServiceProvider_ShouldCreateSeparateInstancesForScoped()
    {
        // Act - Get instances from two different scopes
        var repository1 = _serviceProvider.GetService<IUserRepository>();
        
        using var scope2 = _factory.Services.CreateScope();
        var repository2 = scope2.ServiceProvider.GetService<IUserRepository>();

        // Assert - Different instances but same type
        Assert.NotNull(repository1);
        Assert.NotNull(repository2);
        Assert.IsType<InMemoryUserRepository>(repository1);
        Assert.IsType<InMemoryUserRepository>(repository2);
        Assert.NotSame(repository1, repository2);
    }

    [Fact]
    public void ServiceProvider_ShouldResolveDependenciesCorrectly()
    {
        // Act - Resolve handler which depends on repository and hasher
        var handler = _serviceProvider.GetService<RegisterUserHandler>();

        // Assert - Should resolve successfully with all dependencies
        Assert.NotNull(handler);
        
        // Verify dependencies can be resolved independently
        var repository = _serviceProvider.GetService<IUserRepository>();
        var hasher = _serviceProvider.GetService<IPasswordHasher>();
        Assert.NotNull(repository);
        Assert.NotNull(hasher);
    }

    [Fact]
    public void ServiceProvider_ShouldHandleMultipleResolutionsInSameScope()
    {
        // Act - Get same service multiple times in same scope
        var repository1 = _serviceProvider.GetService<IUserRepository>();
        var repository2 = _serviceProvider.GetService<IUserRepository>();

        // Assert - Should be same instance (Scoped lifetime)
        Assert.NotNull(repository1);
        Assert.NotNull(repository2);
        Assert.Same(repository1, repository2);
    }

    public void Dispose()
    {
        _scope?.Dispose();
        _factory?.Dispose();
    }
}
