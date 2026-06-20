using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        public Guid IdUsers { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;

        public DateTime AccountCreated { get; set; }

        public bool IsActive { get; set; }
    }
}
