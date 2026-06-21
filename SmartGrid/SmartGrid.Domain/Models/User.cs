using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects.User;

namespace SmartGrid.Domain.Models
{
    public class User
    {
        public UserId Id { get; set; }
        public Email Email { get; set; }

        public UserRole Role { get; private set; }
        public PasswordHash Password { get; set; }

        public DateTime AccountCreated { get; set; }

        public ActivationStatus ActivationStatus { get; private set; } = ActivationStatus.NotActivated();

        public User(UserId id, Email email, PasswordHash password)
        {
            Id = id;
            Email = email;
            Password = password;
            AccountCreated = DateTime.UtcNow;
            ActivationStatus = ActivationStatus.NotActivated();
        }

        public User(UserId id, Email email, UserRole role, PasswordHash password, DateTime accountCreated, ActivationStatus activationStatus)
        {
            Id = id;
            Email = email;
            Role = role;
            Password = password;
            AccountCreated = accountCreated;
            ActivationStatus = activationStatus;
        }

        public void Activate()
        {
            ActivationStatus = ActivationStatus.Activated();
        }

        public void Deactivate()
        {
            ActivationStatus = ActivationStatus.NotActivated();
        }

        public void ChangeRole(UserRole role)
        {
            Role = role;
        }

        public static User Create(string email, string password)
        {
            return new User(
                UserId.New(),
                Email.Create(email).Value,
                Domain.Enums.UserRole.User,
                PasswordHash.FromPlainPassword(password),
                DateTime.UtcNow,
                ActivationStatus.NotActivated()
            );
        }

        public static User CreateManaged(string email, string password, UserRole role, bool activated = true)
        {
            return new User(
                UserId.New(),
                Email.Create(email).Value,
                role,
                PasswordHash.FromPlainPassword(password),
                DateTime.UtcNow,
                activated ? ActivationStatus.Activated() : ActivationStatus.NotActivated()
            );
        }
    }
}
