using MediatR;
using Microsoft.Extensions.Logging;
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
    public record ChangeUserRoleCommand(string UserId, string NewRole) : IRequest<Result>;

    public class ChangeUserRoleHandler(IUserRepository userRepository, ILogger<ChangeUserRoleHandler> logger) : IRequestHandler<ChangeUserRoleCommand, Result>
    {
        public async Task<Result> Handle(ChangeUserRoleCommand request, CancellationToken ct)
        {
            if (request == null)
            {
                logger.LogError("ChangeUserRoleCommand request is null.");
                return Result.Failure("Request cannot be null.");
            }

            try 
            {

                var userId = UserId.FromString(request.UserId);
                var user = await userRepository.GetByIdAsync(userId, ct);
                
                if (user == null)
                {
                    logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                    return Result.Failure("User not found.");
                }

                if (!Enum.TryParse(request.NewRole, true, out UserRole parsedRole))
                {
                    logger.LogWarning("Invalid role {NewRole} provided.", request.NewRole);
                    return Result.Failure("Invalid role.");
                }

                user.ChangeRole(parsedRole);
                await userRepository.UpdateAsync(user, ct);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while changing user role for UserId: {UserId}", request.UserId);
                return Result.Failure("An error occurred while changing the user role.");
            }
        }
    }
}
