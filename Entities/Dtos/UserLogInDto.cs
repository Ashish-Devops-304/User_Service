using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class UserLogInDto
    {
        [Required(ErrorMessage = "This field is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        public string Password { get; set; } = string.Empty;

    }
}
