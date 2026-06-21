using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Common;
using SmartGrid.Application.Features.ManualReadings.Commands;
using SmartGrid.Application.Features.ManualReadings.Queries;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Enums;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.Controllers
{
    [Route("api/admin/manual-readings")]
    [ApiController]
    [Authorize(Roles = "Admin,SysAdmin")]
    public class AdminManualReadingsController(
        IMediator mediator,
        IManualReadingRepository manualReadingRepository,
        IManualReadingImageStorage manualReadingImageStorage) : ControllerBase
    {
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending(CancellationToken ct)
        {
            var result = await mediator.Send(new GetManualReadingsQuery(ManualReadingStatus.Pending), ct);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}/image")]
        public async Task<IActionResult> GetImage(Guid id, [FromQuery] string variant = "optimized", CancellationToken ct = default)
        {
            var reading = await manualReadingRepository.GetByIdAsync(id, ct);
            if (reading is null)
            {
                return NotFound(new { message = "Manual reading not found." });
            }

            var normalizedVariant = string.Equals(variant, "raw", StringComparison.OrdinalIgnoreCase)
                ? "raw"
                : "optimized";

            var extension = normalizedVariant == "raw"
                ? Path.GetExtension(reading.RawImagePath).Trim('.')
                : "jpg";

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = "jpg";
            }

            var metadata = new ManualReadingImageMetadata
            {
                ReadingId = id,
                Variant = normalizedVariant,
                FileExtension = extension
            };

            if (!await manualReadingImageStorage.ExistsAsync(metadata, ct))
            {
                return NotFound(new { message = "Image not found." });
            }

            var bytes = await manualReadingImageStorage.ReadAsync(metadata, ct);
            var contentType = extension.Equals("png", StringComparison.OrdinalIgnoreCase)
                ? "image/png"
                : "image/jpeg";

            return File(bytes, contentType);
        }

        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
        {
            var result = await mediator.Send(new ApproveManualReadingCommand(id), ct);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id, CancellationToken ct)
        {
            var result = await mediator.Send(new RejectManualReadingCommand(id), ct);
            return result.ToActionResult();
        }
    }
}
