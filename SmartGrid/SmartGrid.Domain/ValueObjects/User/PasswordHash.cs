namespace SmartGrid.Domain.ValueObjects.User
{
    public class PasswordHash
    {

        public string Value { get; }

        public PasswordHash(string hash)
        {
            Value = hash;
        }

        public static PasswordHash FromPlainPassword(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.", nameof(plainPassword));

            var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            return new PasswordHash(hash);
        }

        public static PasswordHash FromHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Hash cannot be empty.", nameof(hash));

            return new PasswordHash(hash);
        }
        public bool Verify(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, Value);
        }

        public override string ToString() => Value;

        public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;

        public bool Equals(PasswordHash? other)
            => other is not null && Value == other.Value;

        public override bool Equals(object? obj)
            => obj is PasswordHash other && Equals(other);


        public static bool operator ==(PasswordHash left, PasswordHash right)
            => Equals(left, right);

        public override int GetHashCode()
           => Value != null ? Value.GetHashCode() : 0;

        public static bool operator !=(PasswordHash left, PasswordHash right)
            => !Equals(left, right);
    }
}

