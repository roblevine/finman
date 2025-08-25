using Xunit;
using Xunit.Abstractions;

namespace UserService.Tests.Infrastructure.Tests;

public class SwitchableModeTests : SwitchableWebApplicationFactoryTests
{
    private readonly ITestOutputHelper _output;

    public SwitchableModeTests(PostgreSqlFixture fixture, ITestOutputHelper output) : base(fixture)
    {
        _output = output;
    }

    [Fact]
    public void SwitchableMode_ShouldDetect_CurrentMode()
    {
        // Arrange & Act
        var useTestcontainers = Environment.GetEnvironmentVariable("USE_TESTCONTAINERS");
        
        // Assert & Report
        _output.WriteLine($"USE_TESTCONTAINERS environment variable: {useTestcontainers ?? "not set"}");
        _output.WriteLine($"IsUsingPostgreSQL: {IsUsingPostgreSQL}");
        _output.WriteLine($"Expected mode: {(IsUsingPostgreSQL ? "PostgreSQL" : "InMemory")}");
        
        if (IsUsingPostgreSQL)
        {
            Assert.True(IsUsingPostgreSQL, "Should be using PostgreSQL when USE_TESTCONTAINERS=true");
        }
        else
        {
            Assert.False(IsUsingPostgreSQL, "Should be using InMemory when USE_TESTCONTAINERS is not set or false");
        }
    }
}