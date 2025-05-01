using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Payment : BaseModel
    {
        //Below lines establishes a relationship between the tables, "Address" and "User"
        [ForeignKey("UserId")]

        public User User { get; set; }

        public Guid UserId { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string PaymentValue { get; set; } = string.Empty;

        public string? Expiry { get; set; } 
    }
}
