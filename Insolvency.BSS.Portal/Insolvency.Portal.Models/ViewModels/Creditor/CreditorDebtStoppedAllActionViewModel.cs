using System;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorDebtStoppedAllActionViewModel
    {
        public CreditorDebtPartialViewModel Debt { get; set; }
        public Guid DebtId { get; set; }
    }
}
