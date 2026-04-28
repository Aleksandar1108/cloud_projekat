using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories;

namespace SmartGrid.Application.Features.Users.Command
{
    public record LoginUserCommand(
           string Email,
           string Password
       ) : IRequest<Result<AuthResponse>>;
    internal class LoginUserHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtService,
    ILogger<LoginUserHandler> logger)
    : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken ct)
        {
            try
            {
                var existing = await userRepository.GetByEmailAsync(request.Email);
                if (existing is null)
                    return Result<AuthResponse>.Failure("User does not exist", ErrorType.NotFound);

                if (!existing.Password.Verify(request.Password))
                    return Result<AuthResponse>.Failure("Invalid password", ErrorType.Unauthorized);

                var token = jwtService.GenerateToken(existing);

                return Result<AuthResponse>.Success(new AuthResponse(token, DateTime.UtcNow.AddHours(2)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Login failed");
                return Result<AuthResponse>.Failure("Login failed", ErrorType.Failure);
            }
        }
    }
}
