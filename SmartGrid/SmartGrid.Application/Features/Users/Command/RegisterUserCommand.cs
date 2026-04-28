using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories;

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
    ILogger<RegisterUserHandler> logger)
    : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
    {
        public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            try
            {
                var existing = await userRepository.GetByEmailAsync(request.Email);
                if (existing is not null)
                    return Result<AuthResponse>.Failure("User already exists", ErrorType.Conflict);

                var user = User.Create(request.Email, request.Password);

                await userRepository.AddAsync(user);


                var activation = EmailActivation.Create(user.Id);

                await emailActivationRepository.AddAsync(activation);

                var token = jwtService.GenerateToken(user);

                await emailService.SendEmailAsync(user.Email, "Activate your SmartGrid account", activation.Token.Value);

                return Result<AuthResponse>.Success(new AuthResponse(token, DateTime.UtcNow.AddHours(2)));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registration failed");
                return Result<AuthResponse>.Failure("Registration failed", ErrorType.Failure);
            }
        }
    }
}
