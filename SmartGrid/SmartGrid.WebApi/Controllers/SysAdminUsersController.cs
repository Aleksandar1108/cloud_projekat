using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Users.Command;
using SmartGrid.Application.Features.Users.Queries;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "SysAdmin")]
    public class SysAdminUsersController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var result = await mediator.Send(new GetUsersQuery(), ct);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDTO request, CancellationToken ct)
        {
            if (request is null)
            {
                return BadRequest(new { message = "Invalid or empty JSON payload." });
            }

            var command = new CreateUserCommand(request.Email, request.Password, request.Role);
            var result = await mediator.Send(command, ct);
            return result.ToActionResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id, CancellationToken ct)
        {
            var result = await mediator.Send(new DeleteUserCommand(id), ct);
            return result.ToActionResult();
        }

        [HttpPost("{id}/suspend")]
        public async Task<IActionResult> SuspendUser([FromRoute] string id, CancellationToken ct)
        {
            var result = await mediator.Send(new SuspendUserCommand(id), ct);
            return result.ToActionResult();
        }
    }
}
