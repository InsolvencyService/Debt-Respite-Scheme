using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.Shared.Requests
{
    public class DebtSoldOnRequest
    {
        [JsonIgnore]
        public Guid DebtId { get; set; }
        [JsonIgnore]
        public virtual Guid? CreditorId { get; set; }
        [JsonIgnore]
        public virtual Guid? MoneyAdviserId { get; set; }

        [Required(ErrorMessage = "soldToCreditorId is required")]
        public Guid? SoldToCreditorId { get; set; }

        public Dictionary<string, object> ToDictionary() =>
            new Dictionary<string, object>
            {
                {nameof(SoldToCreditorId),SoldToCreditorId.ToString() },
                {nameof(CreditorId), CreditorId?.ToString() },
                {nameof(MoneyAdviserId), MoneyAdviserId?.ToString() }
            };
    }
}
