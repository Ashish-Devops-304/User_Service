namespace Entities.Models
{
    // User - Contains user information

    public class User : BaseModel
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public long Phone { get; set; }

        public string Role { get; set; } = string.Empty;

        public ICollection<Address>? Address { get; set; } = new List<Address>();

        public ICollection<Payment>? Payment { get; set; } = new List<Payment>();
    }
}
