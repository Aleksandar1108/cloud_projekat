using FluentValidation;
using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.SmartMeters.Commands
{
    public record UpdateSmartMeterCommand(
        Guid Id,
        Guid PropertyId,
        Guid UserId,
        string Label,
        ConnectionType ConnectionType,
        string? Note
    ) : IRequest<Result>;

    public class UpdateSmartMeterValidator : AbstractValidator<UpdateSmartMeterCommand>
    {
        public UpdateSmartMeterValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Label).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ConnectionType).IsInEnum();
            RuleFor(x => x.Note).MaximumLength(500).When(x => x.Note != null);
        }
    }

    internal class UpdateSmartMeterHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository) : IRequestHandler<UpdateSmartMeterCommand, Result>
    {
        public async Task<Result> Handle(UpdateSmartMeterCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
                return Result.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            var meter = await smartMeterRepository.GetByIdAsync(request.Id, ct);
            if (meter is null || meter.PropertyId != request.PropertyId)
                return Result.Failure("Smart meter not found.", ErrorType.NotFound);

            meter.Update(request.Label, request.ConnectionType, request.Note);
            await smartMeterRepository.UpdateAsync(meter, ct);

            return Result.Success();
        }
    }
}
