using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Users.Command;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers;

[Route("users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] UserRequestDTO request)
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Password
        );

        var result = await mediator.Send(command);

        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
    [FromBody] UserRequestDTO request)
    {
        var command = new LoginUserCommand(
            request.Email,
            request.Password
        );

        var result = await mediator.Send(command);

        return result.ToActionResult();
    }
}