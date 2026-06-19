using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Application.Features.Users.Command
{
    public record SetUserSuspensionCommand(string UserId, bool Suspend) : IRequest<Result>;

    internal class SetUserSuspensionHandler(
        IUserRepository userRepository,
        ILogger<SetUserSuspensionHandler> logger)
        : IRequestHandler<SetUserSuspensionCommand, Result>
    {
        public async Task<Result> Handle(SetUserSuspensionCommand request, CancellationToken ct)
        {
            try
            {
                var userId = UserId.FromString(request.UserId);
                var user = await userRepository.GetByIdAsync(userId, ct);

                if (user is null)
                {
                    logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                    return Result.Failure("User not found.", ErrorType.NotFound);
                }

                if (request.Suspend)
                {
                    user.Suspend();
                }
                else
                {
                    user.Reactivate();
                }

                await userRepository.UpdateAsync(user, ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while changing suspension state for UserId: {UserId}", request.UserId);
                return Result.Failure("An error occurred while updating the user.", ErrorType.Failure);
            }
        }
    }
}
