using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class CreditorSearchViewModel
    {
        [Required(ErrorMessage = "Creditor name is required")]
        [MaxLength(100)]
        [Display(Name = "Enter the creditor’s name")]
        public virtual string CreditorName { get; set; }
        public Guid? CreditorId { get; set; }
        public Guid DebtId { get; set; }
        public string ReturnAction { get; set; }
        public bool IsEdit { get; set; }
    }
}
