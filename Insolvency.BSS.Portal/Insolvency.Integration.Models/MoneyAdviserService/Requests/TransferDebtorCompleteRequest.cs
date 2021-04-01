using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class TransferDebtorCompleteRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }

        [JsonIgnore]
        public Guid MoneyAdviceOrganisationId { get; set; }

        [JsonIgnore]
        public string MoneyAdviceOrganisationName { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                {"CompletingOrganisationId", MoneyAdviceOrganisationId.ToString() },
                {"CompletingMoneyAdviserName", MoneyAdviceOrganisationName }
            };
        }
    }
}
