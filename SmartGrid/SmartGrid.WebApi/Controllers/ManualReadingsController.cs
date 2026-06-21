using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.ManualReadings.Commands;
using SmartGrid.Application.Features.ManualReadings.Queries;
using SmartGrid.Domain.Enums;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/manual-readings")]
    [ApiController]
    public class ManualReadingsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Submit([FromForm] ManualReadingRequestDto request)
        {
            if (request.MeterImage is null || request.MeterImage.Length == 0)
            {
                return BadRequest(new { message = "Meter image is required." });
            }

            var command = new SubmitManualReadingCommand(
                request.MeterName,
                request.ReadingKwh,
                request.ReadingAtUtc == default ? DateTime.UtcNow : request.ReadingAtUtc,
                request.SubmitterEmail,
                request.MeterImage.FileName,
                await request.MeterImage.ToByteArrayAsync(HttpContext.RequestAborted));

            var result = await mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? status)
        {
            ManualReadingStatus? parsed = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<ManualReadingStatus>(status, true, out var parsedStatus))
            {
                parsed = parsedStatus;
            }

            var result = await mediator.Send(new GetManualReadingsQuery(parsed));
            return result.ToActionResult();
        }
    }
}
