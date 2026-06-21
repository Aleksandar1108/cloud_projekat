using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Application.Features.Users.Command
{
    public record SuspendUserCommand(string UserId) : IRequest<Result>;

    internal sealed class SuspendUserHandler(
        IUserRepository userRepository,
        ILogger<SuspendUserHandler> logger)
        : IRequestHandler<SuspendUserCommand, Result>
    {
        public async Task<Result> Handle(SuspendUserCommand request, CancellationToken ct)
        {
            try
            {
                var userId = UserId.FromString(request.UserId);
                var user = await userRepository.GetByIdAsync(userId, ct);

                if (user is null)
                {
                    return Result.Failure("User not found.", ErrorType.NotFound);
                }

                if (!user.ActivationStatus.Value)
                {
                    return Result.Failure("User is already suspended.", ErrorType.Validation);
                }

                user.Deactivate();
                await userRepository.UpdateAsync(user, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to suspend user {UserId}.", request.UserId);
                return Result.Failure("Failed to suspend user.", ErrorType.Failure);
            }
        }
    }
}
