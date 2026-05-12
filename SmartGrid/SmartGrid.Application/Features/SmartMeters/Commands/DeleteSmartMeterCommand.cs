using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.SmartMeters.Commands
{
    public record DeleteSmartMeterCommand(Guid Id, Guid PropertyId, Guid UserId) : IRequest<Result>;

    internal class DeleteSmartMeterHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository) : IRequestHandler<DeleteSmartMeterCommand, Result>
    {
        public async Task<Result> Handle(DeleteSmartMeterCommand request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
                return Result.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result.Failure("Access denied.", ErrorType.Unauthorized);

            var meter = await smartMeterRepository.GetByIdAsync(request.Id, ct);
            if (meter is null || meter.PropertyId != request.PropertyId)
                return Result.Failure("Smart meter not found.", ErrorType.NotFound);

            await smartMeterRepository.DeleteAsync(request.Id, ct);
            return Result.Success();
        }
    }
}
