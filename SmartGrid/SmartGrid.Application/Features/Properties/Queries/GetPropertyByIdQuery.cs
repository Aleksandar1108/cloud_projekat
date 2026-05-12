using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Properties.Queries
{
    public record GetPropertyByIdQuery(Guid Id, Guid UserId) : IRequest<Result<PropertyDto>>;

    internal class GetPropertyByIdHandler(
        IPropertyRepository propertyRepository) : IRequestHandler<GetPropertyByIdQuery, Result<PropertyDto>>
    {
        public async Task<Result<PropertyDto>> Handle(GetPropertyByIdQuery request, CancellationToken ct)
        {
            var property = await propertyRepository.GetByIdAsync(request.Id, ct);
            if (property is null)
                return Result<PropertyDto>.Failure("Property not found.", ErrorType.NotFound);

            if (property.UserId != request.UserId)
                return Result<PropertyDto>.Failure("Access denied.", ErrorType.Unauthorized);

            return Result<PropertyDto>.Success(new PropertyDto(
                property.Id, property.UserId, property.Name, property.City, property.Address,
                property.Description, property.PropertyType, property.CreatedAt
            ));
        }
    }
}
