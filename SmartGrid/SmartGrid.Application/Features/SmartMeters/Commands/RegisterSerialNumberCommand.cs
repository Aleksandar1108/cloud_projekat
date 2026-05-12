using FluentValidation;
using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.SmartMeters.Commands
{
    public record RegisterSerialNumberCommand(
        Guid SmartMeterId,
        Guid PropertyId,
        Guid UserId,
        string SerialNumber
    ) : IRequest<Result>;

    public class RegisterSerialNumberValidator : AbstractValidator<RegisterSerialNumberCommand>
    {
        public RegisterSerialNumberValidator()
        {
            RuleFor(x => x.SmartMeterId).NotEmpty();
            RuleFor(x => x.PropertyId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.SerialNumber)
                .NotEmpty()
                .Matches(@"^SA-\d{4}-\d{5}$")
                .WithMessage("Serial number must be in format SA-YYYY-XXXXX.");
        }
    }

    internal class RegisterSerialNumberHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository) : IRequestHandler<RegisterSerialNumberCommand, Result>
    {
        public async Task<Result> Handle(RegisterSerialNumberCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
                return Result.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            var meter = await smartMeterRepository.GetByIdAsync(request.SmartMeterId, ct);
            if (meter is null || meter.PropertyId != request.PropertyId)
                return Result.Failure("Smart meter not found.", ErrorType.NotFound);

            var existing = await smartMeterRepository.GetBySerialNumberAsync(request.SerialNumber, ct);
            if (existing is not null && existing.Id != request.SmartMeterId)
                return Result.Failure("Serial number already registered to another device.", ErrorType.Conflict);

            meter.RegisterSerialNumber(request.SerialNumber);
            await smartMeterRepository.UpdateAsync(meter, ct);

            return Result.Success();
        }
    }
}
