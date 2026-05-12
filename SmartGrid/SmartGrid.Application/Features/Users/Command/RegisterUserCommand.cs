using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;

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
                var emailResult = Email.Create(request.Email);
                if (emailResult.IsFailure)
                    return Result<AuthResponse>.Failure(emailResult.Error!.Message, ErrorType.Validation);

                var existing = await userRepository.GetByEmailAsync(request.Email);
                if (existing is not null)
                    return Result<AuthResponse>.Failure("User already exists", ErrorType.Conflict);

                var user = User.Create(request.Email, request.Password);

                await userRepository.AddAsync(user);


                var activation = EmailActivation.Create(user.Id);

                await emailActivationRepository.AddAsync(activation);

                var token = jwtService.GenerateToken(user);

                try
                {
                    await emailService.SendEmailAsync(user.Email, "Activate your SmartGrid account", activation.Token.Value);
                }
                catch (Exception mailEx)
                {
                    logger.LogWarning(mailEx, "Activation email was not sent for user {Email}", user.Email);
                }

                return Result<AuthResponse>.Success(new AuthResponse(token, DateTime.UtcNow.AddHours(2)));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registration failed");
                var reason = ex.GetBaseException().Message;
                if (reason.Length > 400)
                    reason = reason[..400] + "...";
                return Result<AuthResponse>.Failure($"Registration failed: {reason}", ErrorType.Failure);
            }
        }
    }
}
