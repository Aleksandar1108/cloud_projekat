using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ConsumptionLimits.Commands
{
    public record DeleteConsumptionLimitCommand(
        Guid PropertyId,
        Guid SmartMeterId,
        Guid UserId) : IRequest<Result>;

    internal class DeleteConsumptionLimitHandler(
        IPropertyRepository propertyRepository,
        ISmartMeterRepository smartMeterRepository,
        IConsumptionLimitRepository consumptionLimitRepository) : IRequestHandler<DeleteConsumptionLimitCommand, Result>
    {
        public async Task<Result> Handle(DeleteConsumptionLimitCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
            {
                return Result.Failure("Property not found.", ErrorType.NotFound);
            }

            if (property.UserId != request.UserId)
            {
                return Result.Failure("Access denied.", ErrorType.Unauthorized);
            }

            var meter = await smartMeterRepository.GetByIdAsync(request.SmartMeterId, ct);
            if (meter is null || meter.PropertyId != request.PropertyId)
            {
                return Result.Failure("Smart meter not found.", ErrorType.NotFound);
            }

            await consumptionLimitRepository.DeleteAsync(request.UserId, request.SmartMeterId, ct);
            return Result.Success();
        }
    }
}
