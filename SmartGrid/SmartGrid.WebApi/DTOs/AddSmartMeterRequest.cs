using SmartGrid.Domain.Enums;

namespace SmartGrid.WebApi.DTOs
{
    public record AddSmartMeterRequest(
        string Label,
        ConnectionType ConnectionType,
        string? Note
    );
}
