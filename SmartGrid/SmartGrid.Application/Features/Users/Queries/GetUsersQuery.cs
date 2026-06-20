using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.Users.Queries
{
    public record GetUsersQuery()
    : IRequest<Result<IEnumerable<UserDTO>>?>;

    internal class GetUsersHandler(
        IUserRepository userRepository,
        ILogger<GetUsersHandler> logger)
        : IRequestHandler<GetUsersQuery, Result<IEnumerable<UserDTO>>?>
    {
    public async Task<Result<IEnumerable<UserDTO>>?> Handle(GetUsersQuery request,CancellationToken ct)
    {
        try
        {
            var users = await userRepository.GetAllAsync(ct);
            var usersDTO = users.Select(user => new UserDTO
            {
                IdUser = user.Id.Value.ToString(),
                Email = user.Email.Value,
                Role = user.Role.ToString(),
                CreatedAt = user.AccountCreated.ToString("yyyy-MM-dd"),
                IsActivated = user.ActivationStatus.Value
            }).ToList();

            return Result<IEnumerable<UserDTO>>
                .Success(usersDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Error while retrieving users.");
            return Result<IEnumerable<UserDTO>>.Failure("Failed to retrieve users.", ErrorType.Failure);
        }
    }
    }
}
