using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Application.Features.Users.Command
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;
    internal class ForgotPasswordHandler(
    IEmailActivationRepository emailRepository,
    IJwtTokenService jwtService,
    IUserRepository userRepository,
    IEmailService emailService,
    ILogger<ForgotPasswordHandler> logger)
    : IRequestHandler<ForgotPasswordCommand, Result>
    {
        public async Task<Result> Handle(
            ForgotPasswordCommand request,
            CancellationToken ct)
        {
            try
            {
                var user = await userRepository.GetByEmailAsync(request.Email, ct);

                if(user is null)
                {
                    logger.LogWarning("No user found with email {Email}", request.Email);
                    return Result.Failure("No account associated with this email.", ErrorType.NotFound);
                }

                user.Deactivate();

                await userRepository.UpdateAsync(user, ct);

                var activation = EmailActivation.Create(user.Id);

                await emailRepository.AddAsync(activation, ct);

                var token = jwtService.GenerateToken(user);

                var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:5173/";
                var link = $"{frontendUrl}set-password?token={activation.Token.Value}";

                await emailService.SendPasswordResetEmailAsync(user.Email, "Reset your SmartGrid password", link, ct);
                return Result<AuthResponse>.Success(new AuthResponse(token, DateTime.UtcNow.AddHours(2)));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing forgot password request for email {Email}", request.Email);

                return Result.Failure("Failed to process forgot password request.", ErrorType.Failure);
            }
        }
    }
}
