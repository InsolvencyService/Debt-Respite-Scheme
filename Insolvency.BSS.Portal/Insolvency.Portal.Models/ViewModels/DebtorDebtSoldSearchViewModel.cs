using System.ComponentModel.DataAnnotations;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorDebtSoldSearchViewModel : CreditorSearchViewModel
    {
        [Required(ErrorMessage = "Please select a creditor name")]
        [MaxLength(100)]
        [Display(Name = "Who have they sold the debt to?")]
        public override string CreditorName { get; set; }
        public DebtViewModel Debt { get; set; }
        public CreditorResponse Creditor { get; set; }
    }
}
