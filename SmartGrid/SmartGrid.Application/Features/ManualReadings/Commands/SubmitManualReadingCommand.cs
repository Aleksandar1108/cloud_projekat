using MediatR;
using SmartGrid.Application.Common;
using SmartGrid.Application.Features.ManualReadings;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ManualReadings.Commands
{
    public record SubmitManualReadingCommand(
        string DeviceId,
        double ReadingKwh,
        DateTime ReadingAtUtc,
        string SubmitterEmail,
        string ImageFileName,
        byte[] ImageContent
    ) : IRequest<Result<ManualReadingDto>>;

    internal class SubmitManualReadingHandler(
        IManualReadingRepository manualReadingRepository,
        IManualReadingImageStorage manualReadingImageStorage,
        IImageOptimizationService imageOptimizationService)
        : IRequestHandler<SubmitManualReadingCommand, Result<ManualReadingDto>>
    {
        public async Task<Result<ManualReadingDto>> Handle(SubmitManualReadingCommand request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId) || request.ReadingKwh <= 0 || request.ImageContent.Length == 0)
            {
                return Result<ManualReadingDto>.Failure("DeviceId, ReadingKwh and image are required.", ErrorType.Validation);
            }

            var readingId = Guid.NewGuid();
            var originalExtension = Path.GetExtension(request.ImageFileName)?.Trim('.').ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(originalExtension))
            {
                originalExtension = "jpg";
            }

            var rawMetadata = new ManualReadingImageMetadata
            {
                ReadingId = readingId,
                Variant = "raw",
                FileExtension = originalExtension
            };

            await manualReadingImageStorage.SaveAsync(new FileData<ManualReadingImageMetadata>
            {
                Metadata = rawMetadata,
                Content = request.ImageContent
            }, ct);

            var optimizedBytes = imageOptimizationService.Optimize(request.ImageContent);
            var optimizedMetadata = new ManualReadingImageMetadata
            {
                ReadingId = readingId,
                Variant = "optimized",
                FileExtension = "jpg"
            };

            await manualReadingImageStorage.SaveAsync(new FileData<ManualReadingImageMetadata>
            {
                Metadata = optimizedMetadata,
                Content = optimizedBytes
            }, ct);

            var created = await manualReadingRepository.CreateAsync(
                new ManualReadingDto(
                    readingId,
                    request.DeviceId,
                    request.ReadingKwh,
                    request.ReadingAtUtc.ToUniversalTime(),
                    request.SubmitterEmail,
                    ManualReadingStatus.Pending,
                    $"{readingId}/raw.{originalExtension}",
                    $"{readingId}/optimized.jpg",
                    DateTime.UtcNow),
                ct);

            return Result<ManualReadingDto>.Success(created);
        }
    }
}
