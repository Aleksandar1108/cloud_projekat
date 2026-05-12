namespace SmartGrid.WebApi.DTOs;

public sealed record SetConsumptionLimitRequest(Guid UserId, Guid DeviceId, decimal LimitKwh);
