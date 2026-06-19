using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Common;
using SmartGrid.Application.Features.ManualReadings.Commands;
using SmartGrid.Application.Features.ManualReadings.Queries;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Enums;
using SmartGrid.WebApi.Authorization;
using SmartGrid.WebApi.DTOs;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/manual-readings")]
    [ApiController]
    [Authorize]
    public class ManualReadingsController(
        IMediator mediator,
        IManualReadingImageStorage imageStorage) : ControllerBase
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
                request.DeviceId,
                request.ReadingKwh,
                request.ReadingAtUtc == default ? DateTime.UtcNow : request.ReadingAtUtc,
                request.SubmitterEmail,
                request.MeterImage.FileName,
                await request.MeterImage.ToByteArrayAsync(HttpContext.RequestAborted));

            var result = await mediator.Send(command);
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.AnyAdmin)]
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

        [Authorize(Roles = Roles.AnyAdmin)]
        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await mediator.Send(new ApproveManualReadingCommand(id));
            return result.ToActionResult();
        }

        [Authorize(Roles = Roles.AnyAdmin)]
        [HttpGet("{id:guid}/image")]
        public async Task<IActionResult> GetImage(Guid id, CancellationToken ct)
        {
            var metadata = new ManualReadingImageMetadata
            {
                ReadingId = id,
                Variant = "optimized",
                FileExtension = "jpg"
            };

            if (!await imageStorage.ExistsAsync(metadata, ct))
            {
                return NotFound();
            }

            var bytes = await imageStorage.ReadAsync(metadata, ct);
            return File(bytes, "image/jpeg");
        }
    }
}
