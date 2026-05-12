using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Security.Cryptography;

namespace SmartGrid.Application.Features.SmartMeters.Commands
{
    public record ActivateSmartMeterCommand(
        string DeviceUUID,
        string SerialNumber
    ) : IRequest<Result<string>>;

    internal class ActivateSmartMeterHandler(
        ISmartMeterRepository smartMeterRepository) : IRequestHandler<ActivateSmartMeterCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(ActivateSmartMeterCommand request, CancellationToken ct)
        {
            if (!Guid.TryParse(request.DeviceUUID, out _))
                return Result<string>.Failure("Invalid device UUID format.", ErrorType.Validation);

            var meter = await smartMeterRepository.GetBySerialNumberAsync(request.SerialNumber, ct);
            if (meter is null)
                return Result<string>.Failure("Smart meter with provided serial number not found.", ErrorType.NotFound);

            if (meter.PairingStatus == PairingStatus.Paired)
                return Result<string>.Failure("Device is already paired.", ErrorType.Conflict);

            var accessToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            meter.Activate(request.DeviceUUID, accessToken);
            await smartMeterRepository.UpdateAsync(meter, ct);

            return Result<string>.Success(accessToken);
        }
    }
}
