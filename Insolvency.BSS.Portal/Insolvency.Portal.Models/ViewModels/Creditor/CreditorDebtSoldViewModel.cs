using System;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorDebtSoldViewModel
    {
        public CreditorDebtPartialViewModel Debt { get; set; }
        public Guid DebtId { get; set; }
        public CreditorResponse Creditor { get; set; }
        public Guid CreditorId { get; set; }
        public bool IsAdHocCreditor { get; set; }
    }
}
