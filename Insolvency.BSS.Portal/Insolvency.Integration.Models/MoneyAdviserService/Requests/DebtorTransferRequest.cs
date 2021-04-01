using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class DebtorTransferRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }
        public Guid AcknowledgingOrganisationId { get; set; }
        public string AcknowledgingMoneyAdviserName { get; set; }

        public IDictionary<string, object> ToDictionary() => new Dictionary<string, object> 
        {
            { nameof(AcknowledgingOrganisationId) , AcknowledgingOrganisationId },
            { nameof(AcknowledgingMoneyAdviserName), AcknowledgingMoneyAdviserName }
        };
    }
}
