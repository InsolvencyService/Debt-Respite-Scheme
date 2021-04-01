using System;
using System.Text.Json.Serialization;
using Insolvency.Common;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class NominatedContactAddress : CustomAddress
    {
        [JsonIgnore]
        public virtual Guid OwnerId { get; set; }
        [JsonIgnore]
        public virtual DateTime? DateFrom { get; set; }
        [JsonIgnore]
        public virtual DateTime? DateTo { get; set; }

        public NominatedContactAddress() { }
        public NominatedContactAddress(CustomAddress address)
        {
            AddressLine1 = address.AddressLine1;
            AddressLine2 = address.AddressLine2;
            County = address.County;
            Postcode = address.Postcode;
            TownCity = address.TownCity;
            Country = address.Country ?? Constants.UkCountryValue;
        }
    }
}
