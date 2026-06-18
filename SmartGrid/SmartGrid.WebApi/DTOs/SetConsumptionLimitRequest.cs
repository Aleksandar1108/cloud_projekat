using SmartGrid.Domain.Enums;

namespace SmartGrid.WebApi.DTOs
{
    public record SetConsumptionLimitRequest(
        ConsumptionLimitUnit Unit,
        double LimitValue);
}
