using System.Security.Cryptography;
using System.Text;

namespace SmartGrid.Domain.ValueObjects.User
{
    public sealed class ActivationToken
    {
        public string Value { get; }
        public DateTime CreatedAt { get; }
        public DateTime ExpiresAt => CreatedAt.AddMinutes(30);

        public ActivationToken(string value)
        {
            Value = value;
            CreatedAt = DateTime.UtcNow;
        }

        private ActivationToken(string value, DateTime createdAt)
        {
            Value = value;
            CreatedAt = createdAt;
        }

        public static ActivationToken FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Activation token cannot be empty.");

            return new ActivationToken(value);
        }
        public static ActivationToken Create(
            string activationMailId,
            string userId)
        {
            var createdAt = DateTime.UtcNow;

            var raw = $"{activationMailId}{userId}{createdAt:O}";

            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha.ComputeHash(bytes);

            //Replace mora da se uradi da zbog problema sa URL-om
            var token = Convert.ToBase64String(hash)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

            return new ActivationToken(token, createdAt);
        }

        public bool IsExpired()
            => DateTime.UtcNow > ExpiresAt;
    }
}
