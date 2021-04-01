using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class ClientAddPreviousNameViewModel
    {
        [Required(ErrorMessageResourceName = "FName_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [StringLength(100, ErrorMessageResourceName = "FName_Length_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessageResourceName = "MName_Length_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Middle name (optional)")]
        public string MiddleName { get; set; }

        [Required(ErrorMessageResourceName = "LName_Required_Validation", ErrorMessageResourceType = typeof(Translations))]
        [StringLength(100, ErrorMessageResourceName = "LName_Length_Validation", ErrorMessageResourceType = typeof(Translations))]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public Guid MoratoriumId { get; set; }
        public Guid PreviousNameId { get; set; }
        public string ReturnAction { get; set; }
    }
}
