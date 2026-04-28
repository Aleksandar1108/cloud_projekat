using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using System.Text.RegularExpressions;

namespace SmartGrid.Domain.ValueObjects.User
{
    public class Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Result<Email> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result<Email>.Failure("Email format is invalid.", ErrorType.Validation);

            var trimmed = value.Trim();

            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(trimmed, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant))
                return Result<Email>.Failure("Invalid Email format.", ErrorType.Validation);

            return Result<Email>.Success(new Email(trimmed));
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
    }
}
