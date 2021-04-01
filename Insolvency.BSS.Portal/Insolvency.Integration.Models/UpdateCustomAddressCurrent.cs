using System;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models
{
    public class UpdateCustomAddressCurrent : UpdateCustomAddress
    {
        [JsonIgnore]
        public override Guid AddressId { get; set; }
        [JsonIgnore]
        public override Guid MoratoriumId { get; set; }
        [JsonIgnore]
        public override DateTime? DateFrom { get; set; }
        [JsonIgnore]
        public override DateTime? DateTo { get; set; }
        [JsonIgnore]
        public override Guid OwnerId { get; set; }
    }
}
