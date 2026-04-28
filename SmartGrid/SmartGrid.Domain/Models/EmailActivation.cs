using SmartGrid.Domain.ValueObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Domain.Models
{
    public class EmailActivation 
    {
        public EmailActivationId Id { get; }
        public UserId UserId { get; }
        public ActivationToken Token { get; }
        public DateTime CreatedAt { get; }
        public DateTime ExpiresAt { get; }

        public EmailActivation(EmailActivationId activationId, UserId userId, ActivationToken token, DateTime createdAt, DateTime expiresAt)
        {
            Id = activationId;
            UserId = userId;
            Token = token;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
        }

        public static EmailActivation Create(UserId userId)
        {
            var activationId = EmailActivationId.New();
            return new EmailActivation(
                activationId,
                userId,
                ActivationToken.Create(activationId.ToString(), userId.ToString()),
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(30)
            );
        }

        public bool IsExpired()
            => DateTime.UtcNow > ExpiresAt;
    }
}
