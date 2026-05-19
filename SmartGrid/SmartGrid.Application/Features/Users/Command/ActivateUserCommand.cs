using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Application.Features.Users.Command
{
    public record ActivateUserCommand(string Token, string password) : IRequest<Result>;
    internal class ActivateUserHandler(
    IEmailActivationRepository emailRepository,
    IUserRepository userRepository,
    ILogger<ActivateUserHandler> logger)
    : IRequestHandler<ActivateUserCommand, Result>
    {
        public async Task<Result> Handle(
            ActivateUserCommand request,
            CancellationToken ct)
        {
            try
            {
                var activation = await emailRepository.GetByTokenAsync(request.Token,ct);

                if (activation is null)
                {
                    return Result.Failure("Invalid activation token.",ErrorType.NotFound);
                }

                if (activation.ExpiresAt < DateTime.UtcNow)
                {
                    return Result.Failure("Activation token expired.",ErrorType.Validation);
                }

                var user = await userRepository.GetByIdAsync(activation.UserId,ct);

                if (user is null)
                {
                    return Result.Failure("User not found.",ErrorType.NotFound);
                }

                user.Activate();
                user.Password = PasswordHash.FromPlainPassword(request.password);

                await userRepository.UpdateAsync(user, ct);

                await emailRepository.DeleteAsync(activation, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error activating account with token {Token}", request.Token);

                return Result.Failure("Failed to activate account.",ErrorType.Failure);
            }
        }
    }
}
