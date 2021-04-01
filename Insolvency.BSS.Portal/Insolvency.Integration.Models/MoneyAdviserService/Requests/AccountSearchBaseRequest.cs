using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class AccountSearchBaseRequest : IValidatableObject, IBornEntity
    {
        public string Surname { get; set; }
        public string BreathingSpaceReference { get; set; }
        public DateTime DateOfBirth { get; set; }

        private bool IsPropertyValid(
            List<ValidationResult> results,
            string propName,
            Func<object> provider
        )
        {
            var attributes = validationsAttrByProp[propName];
            var validationContext = new ValidationContext(this)
            {
                MemberName = propName
            };
            var validationResults = new List<ValidationResult>();

            foreach (var attr in attributes)
            {
                var validationResult = attr.GetValidationResult(provider(), validationContext);

                if (validationResult != null)
                    validationResults.Add(validationResult);
            }

            results.AddRange(validationResults);

            return !validationResults.Any();
        }

        private static IDictionary<string, IEnumerable<ValidationAttribute>> validationsAttrByProp =>
            new Dictionary<string, IEnumerable<ValidationAttribute>>
            {
                {
                    nameof(BreathingSpaceReference),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute() { ErrorMessageResourceName = "Bs_Ref_Required_Validation", ErrorMessageResourceType = typeof(Translations) },
                        new StringLengthAttribute(50) { ErrorMessageResourceName = "Bs_Ref_Length_Validation", ErrorMessageResourceType = typeof(Translations) }
                    }
                },
                {
                    nameof(Surname),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute() { ErrorMessageResourceName = "Api_Surname_Required_Validation", ErrorMessageResourceType = typeof(Translations) } ,
                        new StringLengthAttribute(100) { ErrorMessageResourceName = "Api_Surname_Length_Validation", ErrorMessageResourceType = typeof(Translations) }
                    }
                },
                {
                    nameof(DateOfBirth),  new List<ValidationAttribute>()
                    {
                        new DateOfBirthValidationAttribute()
                    }
                }
            };

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(BreathingSpaceReference) && string.IsNullOrWhiteSpace(Surname) && DateOfBirth == default)
            {
                IsPropertyValid(validationResults, nameof(BreathingSpaceReference), () => BreathingSpaceReference);
                return validationResults;
            }

            if (string.IsNullOrWhiteSpace(BreathingSpaceReference))
            {
                IsPropertyValid(validationResults, nameof(Surname), () => Surname);
                IsPropertyValid(validationResults, nameof(DateOfBirth), () => DateOfBirth);
                return validationResults;
            }

            IsPropertyValid(validationResults, nameof(BreathingSpaceReference), () => BreathingSpaceReference);
            if (!string.IsNullOrWhiteSpace(Surname))
            {
                validationResults.Add(new ValidationResult("Surname not required", new List<string> { nameof(Surname) }));
            }
            if (DateOfBirth != default)
            {
                validationResults.Add(new ValidationResult("Date of birth not required", new List<string> { nameof(DateOfBirth) }));
            }

            return validationResults;
        }

        [JsonIgnore]
        public bool IsValidDateOfBirth { get; set; }

        public virtual DateTime GetBirthDate()
        {
            if (DateOfBirth == default)
                return DateTime.Now;

            return DateOfBirth;
        }
    }
}
