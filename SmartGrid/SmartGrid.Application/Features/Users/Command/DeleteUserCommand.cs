using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Application.Features.Users.Command
{
    public record DeleteUserCommand(string UserId) : IRequest<Result>;

    internal class DeleteUserHandler( IUserRepository userRepository, ILogger<DeleteUserHandler> logger) : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(
            DeleteUserCommand request,
            CancellationToken ct)
        {
            try
            {
                var userId = UserId.FromString(request.UserId);

                var existingUser = await userRepository.GetByIdAsync(userId, ct);

                if (existingUser is null)
                {
                    return Result.Failure("User not found.", ErrorType.NotFound);
                }

                await userRepository.DeleteAsync( userId, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError( ex, "Error deleting user with id {UserId}", request.UserId);

                return Result.Failure("Failed to delete user.",   ErrorType.Failure);
            }
        }
    }
}