using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class PostcodeSearchViewModel
    {
        [Required(ErrorMessage = "Enter postcode")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        public Guid? AddressId { get; set; }
        public Guid? DebtId { get; set; }
        public Guid? BusinessId { get; set; }
        public string ReturnAction { get; set; }
        public string OnPostRedirectAction { get; set; }
    }
}
