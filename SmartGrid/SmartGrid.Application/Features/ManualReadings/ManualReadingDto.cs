using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ManualReadings
{
    public record ManualReadingDto(
        Guid Id,
        string DeviceId,
        double ReadingKwh,
        DateTime ReadingAtUtc,
        string SubmitterEmail,
        ManualReadingStatus Status,
        string RawImagePath,
        string OptimizedImagePath,
        DateTime CreatedAtUtc
    );
}
