using System;
using Insolvency.Common;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class AddressResponse
    {
        private string _country = Constants.UkCountryValue;
        public Guid AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string TownCity { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public virtual string Country
        {
            get => _country;
            set => _country = string.IsNullOrWhiteSpace(value) ? Constants.UkCountryValue : value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var secondAddress = obj as AddressResponse;

            return string.Equals(AddressLine1, secondAddress.AddressLine1) &&
                string.Equals(AddressLine2, secondAddress.AddressLine2) &&
                string.Equals(Country, secondAddress.Country) &&
                string.Equals(County, secondAddress.County) &&
                string.Equals(Postcode, secondAddress.Postcode) &&
                string.Equals(TownCity, secondAddress.TownCity);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
