using MediatR;
using SmartGrid.Domain.Common;

namespace SmartGrid.WebApi.DTOs
{
    public record UserRequestDTO(
        string Email,
        string Password
    ) : IRequest<Result<string>>;
}
