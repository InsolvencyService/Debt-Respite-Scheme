using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class UpdateDebtRequest : CreateDebtRequest
    {
        public Guid DebtId { get; set; }
        public override Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "Amount", Amount },
                { "CreditorId", CreditorId },
                { "DebtTypeId", DebtTypeId },
                { "NIN", NINO },
                { "Reference", Reference },
                { "OtherDebtType", DebtTypeName }
            };
        }
    }
}
