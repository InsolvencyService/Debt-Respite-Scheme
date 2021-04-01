using System;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class MoratoriumTransfer
    {
        public MoratoriumTransfer() { }
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string RequestingOrganisation { get; set; }
        public string RequestedOn { get; set; }
        public string ReasonForTransfer { get; set; }
        public string TransferredBy { get; set; }
        public string TransferredOn { get; set; }
        public int StatusCode { get; set; }
        public string StatusCodeName { get; set; }

        public DebtorTransferResponse ToDebtorTransfer() 
        {
            return new DebtorTransferResponse
            {
                ReasonForTransfer = ReasonForTransfer,
                RequestedOn = RequestedOn?.ToDateTimeOffset(Constants.UkDateHoursAndMinutesFormat),
                RequestingOrganisation = RequestingOrganisation,
                Status = (TransferDebtorRequestStatusCodes)StatusCode,
                TransferredOn = TransferredOn?.ToDateTimeOffset(Constants.UkDateHoursAndMinutesFormat),
                TransferringOrganistion = TransferredBy
            };
        }
    }
}
