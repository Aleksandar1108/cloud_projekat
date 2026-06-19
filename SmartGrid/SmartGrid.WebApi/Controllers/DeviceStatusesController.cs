using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.DeviceStatuses.Queries;
using SmartGrid.WebApi.Authorization;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.AnyAdmin)]
    public class DeviceStatusesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await mediator.Send(new GetDeviceStatusesQuery());

            if (result is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error.");
            }

            return result.ToActionResult();
        }
    }
}
