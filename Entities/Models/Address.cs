using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    // Address - Contains address details of the user

    public class Address : BaseModel
    {
        //Below lines establishes a relationship between the tables, "Address" and "User"
        [ForeignKey("UserId")]

        public User User { get; set; }

        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public long Phone { get; set; }

        public string Line1 { get; set; } = string.Empty;

        public string Line2 { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int Pincode { get; set; }

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

    }
}
