using System.ComponentModel.DataAnnotations;

namespace Entities.Helpers
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;
        private readonly object[] _desiredValues;

        public RequiredIfAttribute(string otherProperty, params object[] desiredvalues)
        {
            _otherProperty = otherProperty;
            _desiredValues = desiredvalues;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherProperty);

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (_desiredValues.Contains(otherPropertyValue))
            {
                if (value == null)
                {
                    return new ValidationResult("This field is required if the payment type is Credit/Debit");
                }
            }

            return ValidationResult.Success;
        }
    }
}
