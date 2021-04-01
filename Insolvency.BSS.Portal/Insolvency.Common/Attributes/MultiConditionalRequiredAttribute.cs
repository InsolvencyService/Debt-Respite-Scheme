using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Attributes
{
    public class MultiConditionalRequiredAttribute : RequiredAttribute
    {
        public string ActionName { get; set; }

        public MultiConditionalRequiredAttribute(string actionName) : base() => ActionName = actionName;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var entity = (IMultiConditionalRequiredValidation)validationContext.ObjectInstance;

            if (entity.Actions[ActionName]())
            {
                if (string.IsNullOrEmpty(value?.ToString()))
                    return new ValidationResult(ErrorMessage, new List<string> { validationContext.MemberName });

                return base.IsValid(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
