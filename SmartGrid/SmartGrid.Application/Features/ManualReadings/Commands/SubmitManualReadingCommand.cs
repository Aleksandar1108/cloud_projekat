using MediatR;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ManualReadings.Commands
{
    public record SubmitManualReadingCommand(
        string MeterName,
        double ReadingKwh,
        DateTime ReadingAtUtc,
        string SubmitterEmail,
        string ImageFileName,
        byte[] ImageContent
    ) : IRequest<Result<ManualReadingDto>>;

    internal class SubmitManualReadingHandler(
        IManualReadingRepository manualReadingRepository,
        IManualReadingImageStorage manualReadingImageStorage,
        IImageOptimizationService imageOptimizationService,
        ISmartMeterRepository smartMeterRepository)
        : IRequestHandler<SubmitManualReadingCommand, Result<ManualReadingDto>>
    {
        public async Task<Result<ManualReadingDto>> Handle(SubmitManualReadingCommand request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.MeterName) || request.ReadingKwh <= 0 || request.ImageContent.Length == 0)
            {
                return Result<ManualReadingDto>.Failure("Naziv brojila, ocitano stanje i slika su obavezni.", ErrorType.Validation);
            }

            var meter = await smartMeterRepository.GetPairedByLabelAsync(request.MeterName, ct);
            if (meter is null || string.IsNullOrWhiteSpace(meter.DeviceUUID))
            {
                return Result<ManualReadingDto>.Failure(
                    "Brojilo sa tim nazivom nije pronadjeno ili nije upareno.",
                    ErrorType.NotFound);
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
                    meter.DeviceUUID,
                    meter.Label,
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
