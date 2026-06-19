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

    public record RegisterUserCommand(
        string Email,
        string Password
    ) : IRequest<Result<AuthResponse>>;
    internal class RegisterUserHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtService,
    IEmailActivationRepository emailActivationRepository,
    IEmailService emailService,
    IHostEnvironment environment,
    ILogger<RegisterUserHandler> logger)
    : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            try
            {
                var existing = await userRepository.GetByEmailAsync(request.Email,ct);
                if (existing is not null)
                    return Result<AuthResponse>.Failure("User already exists", ErrorType.Conflict);

                var user = User.Create(request.Email, request.Password);

                await userRepository.AddAsync(user,ct);


                var activation = EmailActivation.Create(user.Id);

                await emailActivationRepository.AddAsync(activation,ct);

                var token = jwtService.GenerateToken(user);

                var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:5173/";
                var link = $"{frontendUrl}activate?token={activation.Token.Value}";

                await emailService.SendActivationEmailAsync(user.Email, "Activate your SmartGrid account", link, ct);
                return Result<AuthResponse>.Success(new AuthResponse(token, DateTime.UtcNow.AddHours(2)));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registration failed");
                var message = environment.IsDevelopment()
                    ? $"Registration failed: {ex.Message}"
                    : "Registration failed";
                return Result<AuthResponse>.Failure(message, ErrorType.Failure);
            }
        }
    }
}
