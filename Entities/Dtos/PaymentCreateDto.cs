using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class PaymentCreateDto
    {
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Credit|Debit|UPI)$", ErrorMessage = "This field can be either 'Credit' or 'Debit' or 'UPI'")]
        public string Type { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "This field is required")]
        [StringOrInteger("Type")]
        public string PaymentValue { get; set; } = string.Empty;


        [RequiredIf("Type", "Credit", "Debit")]
        [MonthYear]
        public string? Expiry { get; set; } 
    }
}
