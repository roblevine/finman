using Microsoft.AspNetCore.Mvc;

namespace UserService.Infrastructure.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HelloController : ControllerBase
{
    /// <summary>
    /// Simple hello world endpoint for testing API connectivity
    /// </summary>
    /// <returns>A welcome message with timestamp</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HelloResponse), StatusCodes.Status200OK)]
    public ActionResult<HelloResponse> Get()
    {
        var response = new HelloResponse
        {
            Message = "Hello from Finman User Service!",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        return Ok(response);
    }

    /// <summary>
    /// Personalized hello endpoint
    /// </summary>
    /// <param name="name">Name to greet</param>
    /// <returns>A personalized welcome message</returns>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(HelloResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<HelloResponse> GetPersonalized(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Name cannot be empty");
        }

        var response = new HelloResponse
        {
            Message = $"Hello {name}, welcome to Finman User Service!",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        };

        return Ok(response);
    }
}

public record HelloResponse
{
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string Version { get; init; } = string.Empty;
}