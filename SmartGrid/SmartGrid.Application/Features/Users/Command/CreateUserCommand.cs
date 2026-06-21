using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Users.Command
{
    public record CreateUserCommand(
        string Email,
        string Password,
        string Role) : IRequest<Result<UserDTO>>;

    internal sealed class CreateUserHandler(
        IUserRepository userRepository,
        IHostEnvironment environment,
        ILogger<CreateUserHandler> logger)
        : IRequestHandler<CreateUserCommand, Result<UserDTO>>
    {
        public async Task<Result<UserDTO>> Handle(CreateUserCommand request, CancellationToken ct)
        {
            try
            {
                var existing = await userRepository.GetByEmailAsync(request.Email, ct);
                if (existing is not null)
                {
                    return Result<UserDTO>.Failure("User already exists.", ErrorType.Conflict);
                }

                if (!Enum.TryParse(request.Role, true, out UserRole parsedRole))
                {
                    return Result<UserDTO>.Failure("Invalid role.", ErrorType.Validation);
                }

                var user = User.CreateManaged(request.Email, request.Password, parsedRole, activated: true);
                await userRepository.AddAsync(user, ct);

                return Result<UserDTO>.Success(new UserDTO
                {
                    IdUser = user.Id.Value.ToString(),
                    Email = user.Email.Value,
                    Role = user.Role.ToString(),
                    CreatedAt = user.AccountCreated.ToString("yyyy-MM-dd"),
                    IsActivated = user.ActivationStatus.Value
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create user {Email}.", request.Email);
                var message = environment.IsDevelopment()
                    ? $"Failed to create user: {ex.Message}"
                    : "Failed to create user.";
                return Result<UserDTO>.Failure(message, ErrorType.Failure);
            }
        }
    }
}
