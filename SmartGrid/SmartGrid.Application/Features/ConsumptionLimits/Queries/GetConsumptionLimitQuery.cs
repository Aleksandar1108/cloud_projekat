using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ConsumptionLimits.Queries
{
    public record ConsumptionLimitDto(
        ConsumptionLimitUnit Unit,
        double LimitValue,
        DateTime UpdatedAtUtc);

    public record GetConsumptionLimitQuery(
        Guid PropertyId,
        Guid SmartMeterId,
        Guid UserId) : IRequest<Result<ConsumptionLimitDto?>>;

    internal class GetConsumptionLimitHandler(
        IPropertyRepository propertyRepository,
        ISmartMeterRepository smartMeterRepository,
        IConsumptionLimitRepository consumptionLimitRepository) : IRequestHandler<GetConsumptionLimitQuery, Result<ConsumptionLimitDto?>>
    {
        public async Task<Result<ConsumptionLimitDto?>> Handle(GetConsumptionLimitQuery request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
            {
                return Result<ConsumptionLimitDto?>.Failure("Property not found.", ErrorType.NotFound);
            }

            if (property.UserId != request.UserId)
            {
                return Result<ConsumptionLimitDto?>.Failure("Access denied.", ErrorType.Unauthorized);
            }

            var meter = await smartMeterRepository.GetByIdAsync(request.SmartMeterId, ct);
            if (meter is null || meter.PropertyId != request.PropertyId)
            {
                return Result<ConsumptionLimitDto?>.Failure("Smart meter not found.", ErrorType.NotFound);
            }

            var settings = await consumptionLimitRepository.GetAsync(request.UserId, request.SmartMeterId, ct);
            if (settings is null)
            {
                return Result<ConsumptionLimitDto?>.Success(null);
            }

            return Result<ConsumptionLimitDto?>.Success(
                new ConsumptionLimitDto(settings.Unit, settings.LimitValue, settings.UpdatedAtUtc));
        }
    }
}
