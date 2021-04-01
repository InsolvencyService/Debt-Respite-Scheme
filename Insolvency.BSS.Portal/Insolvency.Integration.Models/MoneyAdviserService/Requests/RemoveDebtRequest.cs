using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class RemoveDebtRequest
    {
        [JsonIgnore]
        public Guid DebtId { get; set; }

        [Required]
        [EnumDataType(typeof(DebtRemovalReason))]
        public DebtRemovalReason? Reason { get; set; }

        [Required]
        public string MoneyAdviserNotes { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { nameof(Reason), ((int)Reason.Value).ToString() },
                { nameof(MoneyAdviserNotes), MoneyAdviserNotes }
            };
        }
    }
}
