using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class TransferDebtorRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }

        [JsonIgnore]
        public Guid MoneyAdviceOrganisationId { get; set; }

        [JsonIgnore]
        public string MoneyAdviceOrganisationName { get; set; }

        [Required]
        public string TransferReason { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                {"ReasonForTransfer", TransferReason },
                {"RequestingOrganisationId", MoneyAdviceOrganisationId.ToString() },
                {"RequestingMoneyAdviserName", MoneyAdviceOrganisationName }
            };
        }
    }
}
