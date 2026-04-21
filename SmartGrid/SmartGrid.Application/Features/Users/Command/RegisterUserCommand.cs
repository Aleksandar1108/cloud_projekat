using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Application.Features.Users.Command
{

    public record RegisterUserCommand(
        string Email,
        string Password
    ) : IRequest<Result<AuthResponse>>;
    internal class RegisterUserHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtService,
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

                var token = jwtService.GenerateToken(user);

                return Result<AuthResponse>.Success(new AuthResponse(token, DateTime.UtcNow.AddHours(2)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registration failed");
                return Result<AuthResponse>.Failure("Registration failed",ErrorType.Failure);
            }
        }
    }
}
