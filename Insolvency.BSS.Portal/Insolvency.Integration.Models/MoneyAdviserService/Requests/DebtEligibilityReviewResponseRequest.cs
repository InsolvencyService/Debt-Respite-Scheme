using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class DebtEligibilityReviewResponseRequest
    {
        [JsonIgnore]
        public Guid DebtId { get; set; }
        [Required]
        public bool? RemoveAfterReview { get; set; }
        [Required]
        public string MoneyAdviserNotes { get; set; }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                {"MoneyAdviserNotes", MoneyAdviserNotes }
            };
        }
    }
}
