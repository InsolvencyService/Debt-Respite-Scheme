using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;

namespace Insolvency.Portal.Models
{
    public class AddressWithValidation : Address, IConditionalRegexExpressionValidationTarget
    {
        [Required(ErrorMessage = "Enter building and street")]
        [Display(Name = "Building and street")]
        public override string AddressLine1 { get; set; }

        [Display(Name = "Building and street 2 of 2")]
        public override string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Enter town or city")]
        [Display(Name = "Town or City")]
        public override string TownCity { get; set; }

        [Display(Name = "County (optional)")]
        public override string County { get; set; }

        [ConditionalRegexExpression(
            Constants.UkPostcodeRegex,
            RequiredErrorMessage = "Enter postcode",
            ErrorMessage = "Postcode is not valid",
            MatchTimeoutInMilliseconds = 3000)]
        [Display(Name = "Postcode")]
        public override string Postcode { get; set; }

        [JsonIgnore]
        public virtual bool ConditionalRegexExpressionFlag { get => Country == Constants.UkCountryValue; }

        [Display(Name = "Country")]
        public override string Country { get; set; } = Constants.UkCountryValue;

        public string ReturnAction { get; set; }
    }
}