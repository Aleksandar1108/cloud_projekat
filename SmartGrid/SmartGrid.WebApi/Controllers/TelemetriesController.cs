using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Telemetries.Commands;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class TelemetriesController(IMediator mediator) : ControllerBase
    {
        [HttpPost("ReceiveTelemetry")]
        public async Task<IActionResult> ReceiveTelemetry([FromBody] ProcessTelemetryCommand? command)
        {
            if (command is null)
            {
                return BadRequest(new { error = "Invalid or empty JSON payload." });
            }

            var result = await mediator.Send(command);
            return result.ToActionResult();
        }
    }
}
