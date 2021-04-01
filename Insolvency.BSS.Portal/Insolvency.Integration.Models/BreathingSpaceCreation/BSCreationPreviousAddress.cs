using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationPreviousAddress : CustomAddress
    {
        [JsonIgnore]
        public override Guid OwnerId { get; set; }

        [Required]
        public override DateTime? DateFrom { get; set; }
        [Required]
        public override DateTime? DateTo { get; set; }

        public override string Country { get => base.Country; set => base.Country = value; }
    }
}
