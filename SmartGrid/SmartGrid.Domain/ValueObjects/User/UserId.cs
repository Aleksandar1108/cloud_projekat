using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Domain.ValueObjects.User
{
    public sealed class UserId
    {
        public Guid Value { get; }

        private UserId(Guid value)
        {
            Value = value;
        }

        public static UserId New() => new UserId(Guid.NewGuid());

        public bool Equals(UserId? other)  => other is not null && Value.Equals(other.Value);
        public static UserId FromGuid(Guid value) => new UserId(value);

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(UserId id) => id.Value;
    }
}
