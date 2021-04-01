using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models
{
    public class CustomAddress : IConditionalRegexExpressionValidationTarget
    {
        private string _country = Constants.UkCountryValue;

        [Required(ErrorMessage = "Enter building and street")]
        [Display(Name = "Building and street")]
        public string AddressLine1 { get; set; }

        [Display(Name = "")]
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Enter town or city")]
        [Display(Name = "Town or City")]
        public string TownCity { get; set; }

        [Display(Name = "County")]
        public string County { get; set; }

        [ConditionalRegexExpression(
            Constants.UkPostcodeRegex,
            RequiredErrorMessage = "Enter postcode",
            ErrorMessage = "Postcode is not valid",
            MatchTimeoutInMilliseconds = 3000)]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Enter country")]
        public virtual string Country
        {
            get => _country;
            set => _country = string.IsNullOrWhiteSpace(value) ? Constants.UkCountryValue : value;
        }

        [JsonIgnore]
        public virtual bool ConditionalRegexExpressionFlag => Country == Constants.UkCountryValue;

        [Required(ErrorMessage = "Enter MoratoriumId Id")]
        public virtual Guid OwnerId { get; set; }

        public virtual DateTime? DateFrom { get; set; }
        public virtual DateTime? DateTo { get; set; }

        public virtual IDictionary<string, object> ToNonIdAndDateDictionary() =>
            new Dictionary<string, object>
            {
                { "AddressLine1", AddressLine1 },
                { "AddressLine2", AddressLine2 },
                { "AddressLine3", TownCity },
                { "AddressLine4", County },
                { "AddressCountry", Country },
                { "AddressPostcode", Postcode }
            };

        public virtual IDictionary<string, object> ToDictionary() =>
            new Dictionary<string, object>
            {
                { "AddressLine1", AddressLine1 },
                { "AddressLine2", AddressLine2 },
                { "AddressLine3", TownCity },
                { "AddressLine4", County },
                { "AddressCountry", Country },
                { "AddressPostcode", Postcode },
                { "DateFrom", DateFrom.HasValue ? DateFrom.Value.ToString(Constants.UkDateFormat) : null },
                { "DateTo", DateTo.HasValue ? DateTo.Value.ToString(Constants.UkDateFormat) : null }
            };

        public override bool Equals(object obj) => IsEqual(obj as CustomAddress);

        private bool IsEqual(CustomAddress address)
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