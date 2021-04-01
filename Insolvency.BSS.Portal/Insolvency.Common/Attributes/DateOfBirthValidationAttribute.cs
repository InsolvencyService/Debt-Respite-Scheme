using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Attributes
{
    public class DateOfBirthValidationAttribute : RequiredAttribute
    {
        public override bool RequiresValidationContext => true;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var entity = (IBornEntity)validationContext.ObjectInstance;
            return ValidateDateOfBirth(validationContext, entity);
        }

        protected virtual ValidationResult ValidateDateOfBirth(ValidationContext validationContext, IBornEntity entity)
        {
            // TODO fix localisation
            //var localizer = validationContext.GetService(typeof(IStringLocalizer<DateOfBirthValidationAttribute>)) as IStringLocalizer<DateOfBirthValidationAttribute>;
                var memberNames = new[] { validationContext.MemberName };

            try
            {
                var dateOfBirth = entity.GetBirthDate();
                if (dateOfBirth == default(DateTime))
                {
                    return new ValidationResult("Enter date of birth", memberNames);
                }
                var now = DateTime.Today;
                var age = now.Year - dateOfBirth.Year;
                if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
                    age--;

                if (age < 16)
                {
                    entity.IsValidDateOfBirth = true;
                    return new ValidationResult("Client needs to be over 16 years of age", memberNames);
                }
            }
            catch(ArgumentOutOfRangeException ex)
            {
                return new ValidationResult("Invalid date of birth", memberNames);
            }
            entity.IsValidDateOfBirth = true;
            return ValidationResult.Success;
        }
    }
}
