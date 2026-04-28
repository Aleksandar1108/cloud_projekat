using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
