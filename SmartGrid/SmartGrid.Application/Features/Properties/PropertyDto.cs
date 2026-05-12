using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Properties
{
    public record PropertyDto(
        Guid Id,
        Guid UserId,
        string Name,
        string City,
        string Address,
        string? Description,
        PropertyType PropertyType,
        DateTime CreatedAt
    );
}
