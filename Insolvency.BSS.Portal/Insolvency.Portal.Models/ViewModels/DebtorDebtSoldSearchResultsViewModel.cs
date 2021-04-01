using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorDebtSoldSearchResultsViewModel : CreditorSearchResultsViewModel
    {
        public DebtorDebtSoldSearchResultsViewModel() : base() { }
        public DebtorDebtSoldSearchResultsViewModel(CreditorSearchResultsViewModel resultsViewModel)
        {
            Creditors = resultsViewModel.Creditors;
            SelectedCreditor = resultsViewModel.SelectedCreditor;
        }

        [Required(ErrorMessage = "Please select a creditor name")]
        [Display(Name = "Choose a creditor")]
        public override Guid? SelectedCreditor { get; set; }

        public bool HasHadAnError { get; set; } = false;
    }
}
