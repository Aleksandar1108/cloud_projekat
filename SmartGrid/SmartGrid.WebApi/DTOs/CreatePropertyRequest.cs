using SmartGrid.Domain.Enums;

namespace SmartGrid.WebApi.DTOs
{
    public record CreatePropertyRequest(
        string Name,
        string City,
        string Address,
        string? Description,
        PropertyType PropertyType
    );
}
