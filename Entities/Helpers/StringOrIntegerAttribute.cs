using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Entities.Helpers
{
    public class StringOrIntegerAttribute : ValidationAttribute
    {
        private readonly string _paymentType;

        public StringOrIntegerAttribute(string paymentType)
        {
            _paymentType = paymentType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var paymentTypeProperty = validationContext.ObjectType.GetProperty(_paymentType);

            var paymentTypePropertyValue = paymentTypeProperty.GetValue(validationContext.ObjectInstance);

            if (paymentTypePropertyValue == null)
                return ValidationResult.Success;

            if (paymentTypePropertyValue.Equals("Credit") || paymentTypePropertyValue.Equals("Debit"))
            {
                if (Regex.IsMatch(value.ToString(), @"[a-zA-Z]"))
                    return new ValidationResult("Payment value can contain only numbers if the type is Credit/Debit");
            }

            return ValidationResult.Success;
        }
    }
}
