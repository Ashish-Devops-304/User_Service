using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [MaxLength(30, ErrorMessage = "Only 30 characters are allowed")]
        public string FirstName { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [MaxLength(30, ErrorMessage = "Only 30 characters are allowed")]
        public string LastName { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [EmailAddress (ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [MinLength(8, ErrorMessage = "Password should be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Should contain atleast one lowercase letter, one uppercase letter, one digit and one special character")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please provide a valid phone number")]
        public long Phone { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Customer|Admin)$", ErrorMessage = "The field can be either 'Customer' or 'Admin'")]
        public string Role { get; set; } = string.Empty;


        public ICollection<AddressCreateDto>? Address { get; set; } = new List<AddressCreateDto>();
    }
}
