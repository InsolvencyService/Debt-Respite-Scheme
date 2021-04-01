using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum BreathingSpaceBrowseTask
    {
        [Display(Name = "Debts to review")]
        DebtsToBeReviewed = 0,

        [Display(Name = "Clients to review")]
        ClientsToBeReviewed = 1,

        [Display(Name = "Proposed new debts")]
        NewDebtsProposed = 2,

        [Display(Name = "Sold debts to transfer")]
        DebtsToBeTransferred = 3,

        [Display(Name = "Clients to transfer")]
        ClientsToBeTransferred = 4,

        [Display(Name = "Clients transferred to us")]
        ClientsTransferredToMoneyAdviser = 5
    }
}
