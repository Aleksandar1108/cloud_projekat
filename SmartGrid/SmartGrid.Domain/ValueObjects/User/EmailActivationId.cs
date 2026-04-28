using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Domain.ValueObjects.User
{
        public sealed class EmailActivationId
    {
            public Guid Value { get; }

            private EmailActivationId(Guid value)
            {
                Value = value;
            }

            public static EmailActivationId New() => new EmailActivationId(Guid.NewGuid());

            public bool Equals(EmailActivationId? other) => other is not null && Value.Equals(other.Value);
            public static EmailActivationId FromGuid(Guid value) => new EmailActivationId(value);

            public override string ToString() => Value.ToString();

            public static implicit operator Guid(EmailActivationId id) => id.Value;
        }
}

