using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Exceptions;
using UserService.Application.Ports;
using UserService.Application.UseCases;
using UserService.Domain.Exceptions;

namespace UserService.Infrastructure.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserHandler _registerUserHandler;

    public AuthController(RegisterUserHandler registerUserHandler)
    {
        _registerUserHandler = registerUserHandler;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">User registration details</param>
    /// <returns>Details of the created user account</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        if (request == null)
        {
            return Problem(
                title: "Invalid request",
                detail: "Request body cannot be null",
                statusCode: StatusCodes.Status400BadRequest);
        }

        // Validate model state (DataAnnotations)
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _registerUserHandler.ExecuteAsync(request);

            // Set Location header for RESTful resource creation
            var locationUri = Url.Action("GetUser", "Users", new { id = response.Id });
            return Created($"/api/users/{response.Id}", response);
        }
        catch (UniquenessViolationException ex)
        {
            return Problem(
                title: "User registration failed",
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8");
        }
        catch (ArgumentException ex)
        {
            return Problem(
                title: "Invalid input",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1");
        }
        catch (UserDomainException ex)
        {
            return Problem(
                title: "Domain validation failed",
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                type: "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1");
        }
    }
}
