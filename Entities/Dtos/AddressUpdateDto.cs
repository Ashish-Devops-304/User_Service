using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class AddressUpdateDto
    {
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 30 characters are allowed")]
        public string? Name { get; set; } 


        [RegularExpression(@"^(Home|Work|Friends and Family|Other)$", ErrorMessage = "The status field must be either 'Home' or 'Work' or 'Friends and Family' or 'Other'")]
        public string? Type { get; set; } 


        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please provide a valid phone number")]
        public long? Phone { get; set; }


        public string? Line1 { get; set; } 


        public string? Line2 { get; set; } 


        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 15 characters are allowed")]
        public string? City { get; set; } 


        [RegularExpression(@"^\d{6}$", ErrorMessage = "Please provide a valid pincode")]
        public int? Pincode { get; set; }


        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 15 characters are allowed")]
        public string? State { get; set; } 


        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        [StringLength(30, ErrorMessage = "Only 15 characters are allowed")]
        public string? Country { get; set; } 
    }
}
