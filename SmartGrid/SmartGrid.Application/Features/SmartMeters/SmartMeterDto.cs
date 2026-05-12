using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.SmartMeters
{
    public record SmartMeterDto(
        Guid Id,
        Guid PropertyId,
        string Label,
        ConnectionType ConnectionType,
        double MaxApprovedPower,
        string? Note,
        string? SerialNumber,
        PairingStatus PairingStatus,
        string? DeviceUUID,
        DateTime CreatedAt
    );
}
