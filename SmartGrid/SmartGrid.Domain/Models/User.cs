using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Domain.Models
{
    public class User
    {
        public UserId Id { get; set;  }
        public Email Email { get; set;  }

        public UserRole Role { get; set; }
        public PasswordHash Password { get; set; }

        public DateTime AccountCreated { get;set; }

        public User(UserId id, Email email, PasswordHash password)
        {
            Id = id;
            Email = email;
            Password = password;
            AccountCreated = DateTime.UtcNow;
        }

        public User(UserId id, Email email, UserRole role, PasswordHash password, DateTime accountCreated)
        {
            Id = id;
            Email = email;
            Role = role;
            Password = password;
            AccountCreated = accountCreated;
        }
    }
}
