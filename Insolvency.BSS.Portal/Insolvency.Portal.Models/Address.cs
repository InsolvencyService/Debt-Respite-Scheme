using System;
using System.Text;
using Insolvency.Common;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models
{
    public class Address
    {
        public Address()
        {
        }

        public Address(AddressResponse response)
        {
            if (response is null) return;

            AddressId = response.AddressId;
            AddressLine1 = response.AddressLine1;
            AddressLine2 = response.AddressLine2;
            TownCity = response.TownCity;
            County = response.County;
            Country = response.Country;
            Postcode = response.Postcode;
        }

        public Address(PreviousAddressResponse response)
        {
            if (response is null) return;

            AddressId = response.AddressId;
            AddressLine1 = response.AddressLine1;
            AddressLine2 = response.AddressLine2;
            TownCity = response.TownCity;
            County = response.County;
            Country = response.Country;
            Postcode = response.Postcode;
            DateFrom = response.DateFrom;
            DateTo = response.DateTo;
        }

        public virtual string AddressLine1 { get; set; }

        public virtual string AddressLine2 { get; set; }

        public virtual string TownCity { get; set; }

        public virtual string County { get; set; }

        public virtual string Postcode { get; set; }

        public virtual string Country { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string FormattedDateFrom => DateFrom.HasValue ? DateFrom.Value.ToString("MMMM yyyy") : string.Empty;
        public string FormattedDateTo => DateTo.HasValue ? DateTo.Value.ToString("MMMM yyyy") : string.Empty;

        public bool HasAddressLine2 => !string.IsNullOrEmpty(AddressLine2);
        public bool HasTownCity => !string.IsNullOrEmpty(TownCity);
        public bool HasCounty => !string.IsNullOrEmpty(County);

        public Guid AddressId { get; set; }

        public string ToSingleLineString()
        {
            var builder = new StringBuilder();
            builder.Append(!string.IsNullOrEmpty(AddressLine1) ? $"{AddressLine1}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(AddressLine2) ? $"{AddressLine2}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(TownCity) ? $"{TownCity}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(County) ? $"{County}, " : string.Empty);
            builder.Append(!string.IsNullOrEmpty(Postcode) ? $"{Postcode}" : string.Empty);
            var result = builder.ToString().Trim().Trim(',');
            return result;
        }

        public CustomAddress ToCustomAddress() =>
            new CustomAddress
            {
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                County = County,
                Postcode = Postcode,
                TownCity = TownCity,
                Country = Country ?? Constants.UkCountryValue,
                DateFrom = DateFrom,
                DateTo = DateTo,
            };

        public UpdateCustomAddress ToUpdateCustomAddress(Guid MoratoriumId) =>
            new UpdateCustomAddress
            {
                AddressId = AddressId,
                MoratoriumId = MoratoriumId,
                AddressLine1 = AddressLine1,
                AddressLine2 = AddressLine2,
                County = County,
                Postcode = Postcode,
                TownCity = TownCity,
                Country = Country ?? Constants.UkCountryValue,
                OwnerId = MoratoriumId,
                DateFrom = DateFrom,
                DateTo = DateTo
            };

        public override bool Equals(object obj) => IsEqual(obj as Address);

        private bool IsEqual(Address address)
        {
            if (address is null) return false;

            return string.Equals(address.AddressLine1, AddressLine1, StringComparison.OrdinalIgnoreCase)
                && string.Equals(address.AddressLine2, AddressLine2, StringComparison.OrdinalIgnoreCase)
                && string.Equals(address.TownCity, TownCity, StringComparison.OrdinalIgnoreCase)
                && string.Equals(address.County, County, StringComparison.OrdinalIgnoreCase)
                && string.Equals(address.Country, Country, StringComparison.OrdinalIgnoreCase)
                && string.Equals(address.Postcode, Postcode, StringComparison.OrdinalIgnoreCase);
        }
    }
}
