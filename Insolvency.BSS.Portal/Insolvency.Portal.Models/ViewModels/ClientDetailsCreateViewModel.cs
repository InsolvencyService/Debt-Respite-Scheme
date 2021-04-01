using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Insolvency.Common;
using Insolvency.Common.Attributes;

namespace Insolvency.Portal.Models.ViewModels
{
    public class ClientDetailsCreateViewModel : IBornEntity
    {
        [Required(ErrorMessageResourceName = "FName_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [StringLength(100, ErrorMessageResourceName = "FName_Length_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "First name")]
        public virtual string FirstName { get; set; }

        [StringLength(100, ErrorMessageResourceName = "MName_Length_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Middle name (optional)")]
        public string MiddleName { get; set; }

        [Required(ErrorMessageResourceName = "LName_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [StringLength(100, ErrorMessageResourceName = "LName_Length_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Last name")]
        public virtual string LastName { get; set; }

        public List<ClientName> PreviousNames { get; set; }

        [Required(ErrorMessageResourceName = "DoB_Day_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Range(1, 31, ErrorMessageResourceName = "DoB_Day_Range_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Day")]
        public virtual int? BirthDay { get; set; }

        [Required(ErrorMessageResourceName = "DoB_Month_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Range(1, 12, ErrorMessageResourceName = "DoB_Month_Range_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Month")]
        public virtual int? BirthMonth { get; set; }

        [Required(ErrorMessageResourceName = "DoB_Year_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Range(1880, 2200, ErrorMessageResourceName = "DoB_Year_Range_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Year")]
        public virtual int? BirthYear { get; set; }

        [DateOfBirthValidation]
        public virtual bool IsValidDateOfBirth { get; set; }

        public string AutoFocus { get; set; }
        public string ReturnAction { get; set; }
        public Guid MoratoriumId { get; set; }

        public bool IsFirstNameFocus => string.Equals(AutoFocus, nameof(FirstName), StringComparison.OrdinalIgnoreCase);
        public bool IsBirthDayFocus => string.Equals(AutoFocus, nameof(BirthDay), StringComparison.OrdinalIgnoreCase);

        public virtual DateTime GetBirthDate()
        {
            if (!this.BirthYear.HasValue || !this.BirthMonth.HasValue || !this.BirthDay.HasValue)
            {
                return DateTime.Now;
            }
            return new DateTime(this.BirthYear.Value, this.BirthMonth.Value, this.BirthDay.Value);
        }

        public void MapCurrentName(ClientName fullName)
        {
            this.FirstName = fullName.FirstName;
            this.MiddleName = fullName.MiddleName;
            this.LastName = fullName.LastName;
        }
    }
}
