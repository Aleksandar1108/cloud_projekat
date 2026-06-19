using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Users.Command;
using SmartGrid.Application.Features.Users.Queries;
using SmartGrid.WebApi.Authorization;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;
using System.Linq;

namespace SmartGrid.WebApi.Controllers;

[Route("users")]
[ApiController]
[Authorize(Roles = Roles.SysAdmin)]
public class UsersController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
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

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateAccountRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new ActivateAccountCommand(request.Token);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] ActivateUserRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new ActivateUserCommand(request.Token, request.Password);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new ForgotPasswordCommand(request.Email);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var command = new GetUsersQuery();

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new CreateUserByAdminCommand(request.Email, request.Role);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(
    [FromRoute] string id,
    CancellationToken ct)
    {
        var command = new DeleteUserCommand(id);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpPatch("{id}/suspension")]
    public async Task<IActionResult> SetSuspension([FromRoute] string id, [FromBody] SetSuspensionRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new SetUserSuspensionCommand(id, request.Suspend);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }

    [HttpPatch("{userID}/role")]
    public async Task<IActionResult> ChangeRole([FromRoute] string userId,[FromBody] ChangeRoleRequestDTO request, CancellationToken ct)
    {
        if (request is null)
        {
            return BadRequest(new { message = "Invalid or empty JSON payload." });
        }

        var command = new ChangeUserRoleCommand(userId, request.NewRole);

        var result = await mediator.Send(command, ct);

        return result.ToActionResult();
    }
}