using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Users.Command;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;
using System.Linq;

namespace SmartGrid.WebApi.Controllers;

[Route("users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new RegisterUserCommand(
            request.Email,
            request.Password
        );

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserRequestDTO request)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new LoginUserCommand(
            request.Email,
            request.Password
        );

        var result = await mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateUserRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new ActivateUserCommand(request.Token, request.Password);

        var result = await mediator.Send(command,ct);

        return result.ToActionResult();
    }
}