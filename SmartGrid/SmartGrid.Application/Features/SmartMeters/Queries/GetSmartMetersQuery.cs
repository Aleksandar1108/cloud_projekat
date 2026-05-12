using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.SmartMeters.Queries
{
    public record GetSmartMetersQuery(Guid PropertyId, Guid UserId) : IRequest<Result<IReadOnlyCollection<SmartMeterDto>>>;

    internal class GetSmartMetersHandler(
        ISmartMeterRepository smartMeterRepository,
        IPropertyRepository propertyRepository) : IRequestHandler<GetSmartMetersQuery, Result<IReadOnlyCollection<SmartMeterDto>>>
    {
        public async Task<Result<IReadOnlyCollection<SmartMeterDto>>> Handle(GetSmartMetersQuery request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.PropertyId, ct);
            if (property is null)
                return Result<IReadOnlyCollection<SmartMeterDto>>.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result<IReadOnlyCollection<SmartMeterDto>>.Failure("Access denied.", ErrorType.Unauthorized);

            var meters = await smartMeterRepository.GetByPropertyIdAsync(request.PropertyId, ct);

            var dtos = meters.Select(m => new SmartMeterDto(
                m.Id, m.PropertyId, m.Label, m.ConnectionType, m.MaxApprovedPower,
                m.Note, m.SerialNumber, m.PairingStatus, m.DeviceUUID, m.CreatedAt
            )).ToList();

            return Result<IReadOnlyCollection<SmartMeterDto>>.Success(dtos);
        }
    }
}
