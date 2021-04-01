using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Attributes
{
    public class ConditionalRequiredAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var entity = (IConditionalRequiredValidation)validationContext.ObjectInstance;

            if (entity.ConditionalFlag)
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                    return new ValidationResult(ErrorMessage, new List<string> { validationContext.MemberName } );

                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
