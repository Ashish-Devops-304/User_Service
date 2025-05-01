using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    // User Secret - Contains hashed password of the user

    public class UserSecret : BaseModel
    {
        //Below lines establishes a relationship between the tables, "User Secret" and "User"
        [ForeignKey("UserId")]

        public User User { get; set; }

        public Guid UserId { get; set; }

        public string Password { get; set; } = string.Empty;

    }
}
