using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ManualReadings.Commands
{
    public record RejectManualReadingCommand(Guid ReadingId) : IRequest<Result>;

    internal sealed class RejectManualReadingHandler(
        IManualReadingRepository manualReadingRepository,
        IManualReadingImageStorage manualReadingImageStorage,
        ILogger<RejectManualReadingHandler> logger)
        : IRequestHandler<RejectManualReadingCommand, Result>
    {
        public async Task<Result> Handle(RejectManualReadingCommand request, CancellationToken ct)
        {
            var reading = await manualReadingRepository.GetByIdAsync(request.ReadingId, ct);
            if (reading is null || reading.Status != ManualReadingStatus.Pending)
            {
                return Result.Failure("Manual reading not found.", ErrorType.NotFound);
            }

            await TryDeleteImageAsync(reading.Id, "raw", Path.GetExtension(reading.RawImagePath).Trim('.'), ct);
            await TryDeleteImageAsync(reading.Id, "optimized", "jpg", ct);

            var deleted = await manualReadingRepository.DeletePendingAsync(request.ReadingId, ct);
            if (!deleted)
            {
                return Result.Failure("Manual reading not found.", ErrorType.NotFound);
            }

            logger.LogInformation("Manual reading {ReadingId} rejected and removed.", request.ReadingId);
            return Result.Success();
        }

        private async Task TryDeleteImageAsync(Guid readingId, string variant, string extension, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = "jpg";
            }

            try
            {
                await manualReadingImageStorage.DeleteAsync(new ManualReadingImageMetadata
                {
                    ReadingId = readingId,
                    Variant = variant,
                    FileExtension = extension
                }, ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete {Variant} image for manual reading {ReadingId}.", variant, readingId);
            }
        }
    }
}
