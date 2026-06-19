using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Users.Command
{
    public record CreateUserByAdminCommand(string Email, string Role) : IRequest<Result>;

    internal class CreateUserByAdminHandler(
        IUserRepository userRepository,
        IEmailActivationRepository emailActivationRepository,
        IEmailService emailService,
        IHostEnvironment environment,
        ILogger<CreateUserByAdminHandler> logger)
        : IRequestHandler<CreateUserByAdminCommand, Result>
    {
        public async Task<Result> Handle(CreateUserByAdminCommand request, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Result.Failure("Email is required.", ErrorType.Validation);
                }

                if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
                {
                    return Result.Failure("Invalid role.", ErrorType.Validation);
                }

                var existing = await userRepository.GetByEmailAsync(request.Email, ct);
                if (existing is not null)
                {
                    return Result.Failure("User already exists", ErrorType.Conflict);
                }

                var user = User.CreateByAdmin(request.Email, role);
                await userRepository.AddAsync(user, ct);

                var activation = EmailActivation.Create(user.Id);
                await emailActivationRepository.AddAsync(activation, ct);

                var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:5173/";
                var link = $"{frontendUrl}set-password?token={activation.Token.Value}";

                await emailService.SendActivationEmailAsync(user.Email, "Activate your SmartGrid account", link, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Admin user creation failed");
                var message = environment.IsDevelopment()
                    ? $"User creation failed: {ex.Message}"
                    : "User creation failed";
                return Result.Failure(message, ErrorType.Failure);
            }
        }
    }
}
