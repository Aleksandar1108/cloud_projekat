using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Domain.ValueObjects.User;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Common;
using SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Mappers
{
    public class UserMapper : IDatabaseMapper<User,UserEntity>
    {
        public User? ToDomain(UserEntity entity)
        {
            return new User(
                UserId.FromGuid(Guid.Parse(entity.IdUsers)),
                Email.Create(entity.Email).Value,
                Enum.Parse<Domain.Enums.UserRole>(entity.Role),
                new PasswordHash(entity.Password),
                entity.AccountCreated
            );
        }

        public UserEntity ToEntity(User domain)
        {
            return new UserEntity
            {
                IdUsers = domain.Id.Value.ToString(),
                Email = domain.Email.Value,
                Password = domain.Password.Value,
                Role = domain.Role.ToString(),
                AccountCreated = domain.AccountCreated
            };

        }
    }
}
