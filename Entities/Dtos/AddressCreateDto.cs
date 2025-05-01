using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class AddressCreateDto
    {

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 30 characters are allowed")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Home|Work|Friends and Family|Other)$", ErrorMessage = "The status field must be either 'Home' or 'Work' or 'Friends and Family' or 'Other'")]
        public string Type { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please provide a valid phone number")]
        public long Phone { get; set; }


        [Required(ErrorMessage = "This field is required")]
        public string Line1 { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        public string Line2 { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 15 characters are allowed")]
        public string City { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please provide a valid pincode")]
        public int Pincode { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 15 characters are allowed")]
        public string State { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 15 characters are allowed")]
        public string Country { get; set; } = string.Empty;
    }
}
