using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Domain.ValueObjects.User
{
    public sealed class ActivationStatus
    {
        public bool Value { get; }

        private ActivationStatus(bool value)
        {
            Value = value;
        }

        public static ActivationStatus Activated() => new(true);
        public static ActivationStatus NotActivated() => new(false);
    }
}
