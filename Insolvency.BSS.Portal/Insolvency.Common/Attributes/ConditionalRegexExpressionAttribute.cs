using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Attributes
{
    public class ConditionalRegexExpressionAttribute : RegularExpressionAttribute
    {
        public string RequiredErrorMessage { get; set; }
        public ConditionalRegexExpressionAttribute(string pattern) : base(pattern) { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var entity = (IConditionalRegexExpressionValidationTarget)validationContext.ObjectInstance;
            if (entity.ConditionalRegexExpressionFlag)
            {
                if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
                    return new ValidationResult(RequiredErrorMessage, new List<string> { validationContext.MemberName });

                return base.IsValid(value, validationContext);
            }
            return ValidationResult.Success;
        }
    }
}
