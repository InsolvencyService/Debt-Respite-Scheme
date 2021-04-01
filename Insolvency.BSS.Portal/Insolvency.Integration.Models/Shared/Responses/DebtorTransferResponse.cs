using System;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class DebtorTransferResponse
    {
        public DebtorTransferResponse() { }
        public string RequestingOrganisation { get; set; }
        public DateTimeOffset? RequestedOn { get; set; }
        public DateTimeOffset? TransferredOn { get; set; }
        public string ReasonForTransfer { get; set; }
        public TransferDebtorRequestStatusCodes Status { get; set; }
        public string TransferringOrganistion { get; set; }
    }
}
