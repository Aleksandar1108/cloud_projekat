using MediatR;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;

namespace SmartGrid.Application.Features.Properties.Queries
{
    public record GetPropertiesQuery(Guid UserId) : IRequest<Result<IReadOnlyCollection<PropertyDto>>>;

    internal class GetPropertiesHandler(
        IPropertyRepository propertyRepository) : IRequestHandler<GetPropertiesQuery, Result<IReadOnlyCollection<PropertyDto>>>
    {
        public async Task<Result<IReadOnlyCollection<PropertyDto>>> Handle(GetPropertiesQuery request, CancellationToken ct)
        {
            var properties = await propertyRepository.GetByUserIdAsync(request.UserId, ct);

            var dtos = properties.Select(p => new PropertyDto(
                p.Id, p.UserId, p.Name, p.City, p.Address, p.Description, p.PropertyType, p.CreatedAt
            )).ToList();

            return Result<IReadOnlyCollection<PropertyDto>>.Success(dtos);
        }
    }
}
