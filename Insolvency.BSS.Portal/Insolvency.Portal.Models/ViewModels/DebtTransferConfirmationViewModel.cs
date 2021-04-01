using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtTransferConfirmationViewModel
    {
        public DebtDetailViewModel DebtDetail { get; set; }
        public bool IsAdHocCreditor { get; set; }
        public CreditorResponse DebtSoldToCreditor { get; set; }
    }
}
