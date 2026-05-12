using FluentValidation;
using MediatR;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.SmartMeters.Commands
{
    public record AddSmartMeterCommand(
        Guid PropertyId,
        Guid UserId,
        string Label,
        ConnectionType ConnectionType,
        string? Note
    ) : IRequest<Result<SmartMeterDto>>;

    public class AddSmartMeterValidator : AbstractValidator<AddSmartMeterCommand>
    {
        public AddSmartMeterValidator()
        {
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Label).NotEmpty().WithMessage("Label is required.").MaximumLength(100);
            RuleFor(x => x.ConnectionType).IsInEnum().WithMessage("Invalid connection type.");
            RuleFor(x => x.Note).MaximumLength(500).When(x => x.Note != null);
        }
    }

    internal class AddSmartMeterHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository,
        IDateTimeProvider dateTimeProvider) : IRequestHandler<AddSmartMeterCommand, Result<SmartMeterDto>>
    {
        public async Task<Result<SmartMeterDto>> Handle(AddSmartMeterCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
                return Result<SmartMeterDto>.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result<SmartMeterDto>.Failure("Access denied.", ErrorType.Unauthorized);

            var meter = SmartMeter.Create(request.PropertyId, request.Label, request.ConnectionType, request.Note, dateTimeProvider.UtcNow);

            await smartMeterRepository.AddAsync(meter, ct);

            return Result<SmartMeterDto>.Success(new SmartMeterDto(
                meter.Id, meter.PropertyId, meter.Label, meter.ConnectionType,
                meter.MaxApprovedPower, meter.Note, meter.SerialNumber, meter.PairingStatus,
                meter.DeviceUUID, meter.CreatedAt
            ));
        }
    }
}
