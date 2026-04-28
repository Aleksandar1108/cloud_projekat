using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        public string IdUsers { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;

        public DateTime AccountCreated { get; set; }

        public bool IsActive { get; set; }
    }
}
