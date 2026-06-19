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

        public bool IsSuspended { get; private set; }

        public User(UserId id, Email email, PasswordHash password)
        {
            Id = id;
            Email = email;
            Password = password;
            AccountCreated = DateTime.UtcNow;
            ActivationStatus = ActivationStatus.NotActivated();
            IsSuspended = false;
        }

        public User(UserId id, Email email, UserRole role, PasswordHash password, DateTime accountCreated, ActivationStatus activationStatus, bool isSuspended = false)
        {
            Id = id;
            Email = email;
            Role = role;
            Password = password;
            AccountCreated = accountCreated;
            ActivationStatus = activationStatus;
            IsSuspended = isSuspended;
        }

        public void Activate()
        {
            ActivationStatus = ActivationStatus.Activated();
        }

        public void Deactivate()
        {
            ActivationStatus = ActivationStatus.NotActivated();
        }

        public void Suspend()
        {
            IsSuspended = true;
        }

        public void Reactivate()
        {
            IsSuspended = false;
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

        /// <summary>
        /// Creates a user from the admin panel with an assigned role. The account is
        /// created inactive with a random placeholder password; the user sets their own
        /// password later through the activation link sent by email.
        /// </summary>
        public static User CreateByAdmin(string email, UserRole role)
        {
            return new User(
                UserId.New(),
                Email.Create(email).Value,
                role,
                PasswordHash.FromPlainPassword(Guid.NewGuid().ToString("N")),
                DateTime.UtcNow,
                ActivationStatus.NotActivated()
            );
        }
    }
}
