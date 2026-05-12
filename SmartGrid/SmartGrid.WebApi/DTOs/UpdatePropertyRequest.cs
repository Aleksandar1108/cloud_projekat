using SmartGrid.Domain.Enums;

namespace SmartGrid.WebApi.DTOs
{
    public record UpdatePropertyRequest(
        string Name,
        string City,
        string Address,
        string? Description,
        PropertyType PropertyType
    );
}
