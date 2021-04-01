using System;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationAddress : CustomAddress
    {
        [JsonIgnore]
        public override Guid OwnerId { get; set; }

        [JsonIgnore]
        public override DateTime? DateFrom { get; set; }
        [JsonIgnore]
        public override DateTime? DateTo { get; set; }

        public override string Country { get => base.Country; set => base.Country = value; }
    }
}
