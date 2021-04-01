using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Insolvency.Common;
using Insolvency.Common.Attributes;

namespace Insolvency.Portal.Models.ViewModels
{
    public class AccountSearchViewModel : IValidatableObject, IBornEntity
    {
        [Display(Name = "Reference")]
        public string Reference { get; set; }

        [Display(Name = "Last name")]
        public string Surname { get; set; }

        [Display(Name = "Day")]
        public int? BirthDay { get; set; }

        [Display(Name = "Month")]
        public int? BirthMonth { get; set; }

        [Display(Name = "Year")]
        public int? BirthYear { get; set; }

        [Display(Name = "IsValidDateOfBirth")]
        public bool IsValidDateOfBirth { get; set; }

        public AccountSearchResultViewModel SearchResultViewModel { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        [Required(ErrorMessage = "Please complete all fields")]
        public string SearchOption { get; set; }

        public bool IsSearchByRef => string.Equals(ViewModels.SearchOption.Reference.ToString(), SearchOption, StringComparison.OrdinalIgnoreCase);
        public bool IsSearchByLastnameAndDob => string.Equals(ViewModels.SearchOption.LastnameAndDob.ToString(), SearchOption, StringComparison.OrdinalIgnoreCase);

        public virtual bool HasDateValue()
        {
            if (!BirthYear.HasValue || !BirthMonth.HasValue || !BirthDay.HasValue)
                return false;

            if (BirthYear < DateTime.MinValue.Year || BirthYear > DateTime.MaxValue.Year)
                return false;

            if (BirthMonth < 1 || BirthMonth > 12)
                return false;

            return BirthDay > 0 && BirthDay <= DateTime.DaysInMonth(BirthYear.Value, BirthMonth.Value);
        }

        public string FormattedSearchDate => GetBirthDate().ToString(Constants.UkDateFormat);

        public virtual DateTime GetBirthDate()
        {
            if (!this.HasDateValue())
            {
                return DateTime.Now;
            }

            return new DateTime(this.BirthYear.Value, this.BirthMonth.Value, this.BirthDay.Value);
        }

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
                    nameof(Reference),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute { ErrorMessageResourceName = "Ref_Required_Validation", ErrorMessageResourceType = typeof(Translations) },
                        new StringLengthAttribute(50) { ErrorMessageResourceName = "Ref_Length_Validation", ErrorMessageResourceType = typeof(Translations) },
                        new RegularExpressionAttribute(Constants.BssReferenceNumberRegex) {ErrorMessage = "Incorrect reference number format"}
                    }
                },
                {
                    nameof(Surname),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute() { ErrorMessageResourceName = "Surname_Required_Validation", ErrorMessageResourceType = typeof(Translations) } ,
                        new StringLengthAttribute(100) { ErrorMessageResourceName = "Surname_Length_Validation", ErrorMessageResourceType = typeof(Translations) }
                    }
                },
                {
                    nameof(BirthDay),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute { ErrorMessageResourceName = "DoB_Day_Required_Validation", ErrorMessageResourceType = typeof(Translations) },
                        new RangeAttribute(1, 31) { ErrorMessageResourceName = "DoB_Day_Range_Validation", ErrorMessageResourceType = typeof(Translations) }
                    }
                },
                {
                    nameof(BirthMonth),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute { ErrorMessageResourceName = "DoB_Month_Required_Validation", ErrorMessageResourceType = typeof(Translations) },
                        new RangeAttribute(1, 12) { ErrorMessageResourceName = "DoB_Month_Range_Validation", ErrorMessageResourceType = typeof(Translations) }
                    }
                },
                {
                    nameof(BirthYear),  new List<ValidationAttribute>()
                    {
                        new RequiredAttribute { ErrorMessageResourceName = "DoB_Year_Required_Validation", ErrorMessageResourceType = typeof(Translations) },
                        new RangeAttribute(1880, 2200) { ErrorMessageResourceName = "DoB_Year_Range_Validation", ErrorMessageResourceType = typeof(Translations) }
                    }
                },
                {
                    nameof(IsValidDateOfBirth),  new List<ValidationAttribute>()
                    {
                        new DateOfBirthValidationAttribute()
                    }
                }
            };

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            var isDayValid = false;
            var isMonthValid = false;
            var isYearValid = false;
            var isDobValid = false;
            var isRefValid = false;
            var isSurnameValid = false;

            if (IsSearchByRef)
            {
                isRefValid = IsPropertyValid(validationResults, nameof(Reference), () => this.Reference);

                Surname = string.Empty;
                BirthDay = null;
                BirthMonth = null;
                BirthYear = null;
            }

            if (IsSearchByLastnameAndDob)
            {
                isSurnameValid = IsPropertyValid(validationResults, nameof(Surname), () => this.Surname);
                isDayValid = IsPropertyValid(validationResults, nameof(BirthDay), () => this.BirthDay);
                isMonthValid = IsPropertyValid(validationResults, nameof(BirthMonth), () => this.BirthMonth);
                isYearValid = IsPropertyValid(validationResults, nameof(BirthYear), () => this.BirthYear);
                isDobValid = IsPropertyValid(validationResults, nameof(IsValidDateOfBirth), () => this.IsValidDateOfBirth);

                Reference = null;
            }

            var isRawDayValid = BirthDay == null || BirthDay == default;
            var isRawMonthValid = BirthMonth == null || BirthMonth == default;
            var isRawYearValid = BirthYear == null || BirthYear == default;

            var isRefEmpty = string.IsNullOrEmpty(Reference);
            var isSurnameEmpty = string.IsNullOrEmpty(Surname);
            var isDobEmpty = isRawDayValid && isRawMonthValid && isRawYearValid;
            var isDobValidForSearch = (isDayValid && isMonthValid && isYearValid && isDobValid);


            var isValidForSearch = ((isRefValid
                                        && isSurnameEmpty
                                        && isDobEmpty)
                                    || (isSurnameValid
                                        && isDobValidForSearch
                                        && isRefEmpty)
                                    );

            if (isValidForSearch)
            {
                return Enumerable.Empty<ValidationResult>();
            }

            return validationResults;
        }
    }
}
