using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ManualReadings
{
    public record ManualReadingDto(
        Guid Id,
        string DeviceId,
        string MeterName,
        double ReadingKwh,
        DateTime ReadingAtUtc,
        string SubmitterEmail,
        ManualReadingStatus Status,
        string RawImagePath,
        string OptimizedImagePath,
        DateTime CreatedAtUtc
    );
}
