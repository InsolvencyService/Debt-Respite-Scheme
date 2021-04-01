using System;

namespace Insolvency.Integration.Models.CreditorService.Responses
{
    public class ProposeDebtResponse
    {
        public Guid DebtId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
