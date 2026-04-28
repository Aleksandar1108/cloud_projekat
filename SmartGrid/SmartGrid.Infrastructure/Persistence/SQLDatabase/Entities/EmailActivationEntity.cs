using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGrid.Infrastructure.Persistence.SQLDatabase.Entities
{
    [Table("EmailActivation")]
    public class EmailActivationEntity
    {
        public Guid IdEmailActivation { get; set; }

        public Guid UserId { get; set; }

        public string ActivationToken { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
