using FluentValidation;
using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.ConsumptionLimits.Commands
{
    public record SetConsumptionLimitCommand(
        Guid PropertyId,
        Guid SmartMeterId,
        Guid UserId,
        ConsumptionLimitUnit Unit,
        double LimitValue) : IRequest<Result>;

    public class SetConsumptionLimitValidator : AbstractValidator<SetConsumptionLimitCommand>
    {
        public SetConsumptionLimitValidator()
        {
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.SmartMeterId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.LimitValue).GreaterThan(0);
        }
    }

    internal class SetConsumptionLimitHandler(
        IPropertyRepository propertyRepository,
        ISmartMeterRepository smartMeterRepository,
        IConsumptionLimitRepository consumptionLimitRepository) : IRequestHandler<SetConsumptionLimitCommand, Result>
    {
        public async Task<Result> Handle(SetConsumptionLimitCommand request, CancellationToken ct)
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

            await consumptionLimitRepository.SaveAsync(
                new ConsumptionLimitSettings(
                    request.UserId,
                    request.SmartMeterId,
                    request.Unit,
                    request.LimitValue,
                    DateTime.UtcNow),
                ct);

            return Result.Success();
        }
    }
}
