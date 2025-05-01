using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Entities.Helpers
{
    public class MonthYearAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is string str)
            {
                if (DateTime.TryParseExact(str, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult("This field should be in the format, MM-YYYY (Month-Year)");
        }
    }
}
