using SmartGrid.Domain.Enums;

namespace SmartGrid.WebApi.DTOs
{
    public record UpdateSmartMeterRequest(
        string Label,
        ConnectionType ConnectionType,
        string? Note
    );
}
