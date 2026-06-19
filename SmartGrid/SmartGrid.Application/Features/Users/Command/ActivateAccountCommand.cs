using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Users.Command
{
    /// <summary>
    /// Activates an account using the activation token without changing the password.
    /// Used for the self-registration flow where the user already chose a password.
    /// </summary>
    public record ActivateAccountCommand(string Token) : IRequest<Result>;

    internal class ActivateAccountHandler(
        IEmailActivationRepository emailRepository,
        IUserRepository userRepository,
        ILogger<ActivateAccountHandler> logger)
        : IRequestHandler<ActivateAccountCommand, Result>
    {
        public async Task<Result> Handle(ActivateAccountCommand request, CancellationToken ct)
        {
            try
            {
                var activation = await emailRepository.GetByTokenAsync(request.Token, ct);

                if (activation is null)
                {
                    return Result.Failure("Invalid activation token.", ErrorType.NotFound);
                }

                if (activation.ExpiresAt < DateTime.UtcNow)
                {
                    return Result.Failure("Activation token expired.", ErrorType.Validation);
                }

                var user = await userRepository.GetByIdAsync(activation.UserId, ct);

                if (user is null)
                {
                    return Result.Failure("User not found.", ErrorType.NotFound);
                }

                user.Activate();

                await userRepository.UpdateAsync(user, ct);
                await emailRepository.DeleteAsync(activation, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error activating account with token {Token}", request.Token);
                return Result.Failure("Failed to activate account.", ErrorType.Failure);
            }
        }
    }
}
