using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class PaymentUpdateDto
    {
        [RegularExpression(@"^(Credit|Debit|UPI)$", ErrorMessage = "This field can be either 'Credit' or 'Debit' or 'UPI'")]
        public string? Type { get; set; } 


        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "This field should only contain alphabets")]
        public string? Name { get; set; } 


        [StringOrInteger("Type")]
        public string? PaymentValue { get; set; }


        [MonthYear]
        public string? Expiry { get; set; }
    }
}
