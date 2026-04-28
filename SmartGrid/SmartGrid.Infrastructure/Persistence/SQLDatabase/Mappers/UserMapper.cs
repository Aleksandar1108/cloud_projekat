using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects.User;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Mappers
{
    public class UserMapper : IDatabaseMapper<User, UserEntity>
    {
        public User? ToDomain(UserEntity entity)
        {
            return new User(
                UserId.FromGuid(entity.IdUsers),
                Email.Create(entity.Email).Value,
                Enum.Parse<Domain.Enums.UserRole>(entity.Role),
                new PasswordHash(entity.Password),
                entity.AccountCreated,
                entity.IsActive ? ActivationStatus.Activated() : ActivationStatus.NotActivated()
            );
        }

        public UserEntity ToEntity(User domain)
        {
            return new UserEntity
            {
                IdUsers = domain.Id,
                Email = domain.Email.Value,
                Password = domain.Password.Value,
                Role = domain.Role.ToString(),
                AccountCreated = domain.AccountCreated,
                IsActive = domain.ActivationStatus.Value
            };

        }
    }
}
