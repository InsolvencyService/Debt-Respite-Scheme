using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models
{
    public abstract class AbstractDebtorNominatedContact : IMultiConditionalRequiredValidation
    {
        private string _emailAddress;
        private string _confirmEmailAddress;

        [Required(ErrorMessage = "Please select a point of contacts role")]
        [EnumDataType(typeof(PointContactRoleType))]
        [Display(Name = "Point of contacts role")]
        public PointContactRoleType? PointContactRole { get; set; }

        [Required(ErrorMessage = "Please enter full name")]
        [MaxLength(100, ErrorMessage = "There is a problem Invalid full name length")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [RegularExpression(
            pattern: Constants.BssTelephoneRegex,
            ErrorMessage = @"Enter a telephone number, like +4408081570192",
            MatchTimeoutInMilliseconds = 3000)]
        [Display(Name = "Telephone number (Optional)")]
        public string TelephoneNumber { get; set; }

        [MultiConditionalRequired("Email", ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [Display(Name = "Email address")]
        public string EmailAddress
        {
            get => _emailAddress?.Trim();
            set => _emailAddress = value;
        }

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MultiConditionalRequired("Email", ErrorMessage = "Please enter confirm email address")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [Compare(nameof(EmailAddress), ErrorMessage = "The email addresses you entered do not match")]
        [Display(Name = "Confirm email address")]
        public virtual string ConfirmEmailAddress
        {
            get => _confirmEmailAddress?.Trim();
            set => _confirmEmailAddress = value;
        }

        [Required(ErrorMessage = "Please select a confirmation method")]
        [Display(Name = "Contact confirmation method")]
        [EnumDataType(typeof(PointContactConfirmationMethod))]
        public PointContactConfirmationMethod? ContactConfirmationMethod { get; set; }

        [JsonIgnore]
        public virtual Dictionary<string, Func<bool>> Actions => new Dictionary<string, Func<bool>>
        {
            {"Address", () => ContactConfirmationMethod.HasValue && ContactConfirmationMethod.Value == PointContactConfirmationMethod.Post},
            {"Email", () => ContactConfirmationMethod.HasValue && ContactConfirmationMethod.Value == PointContactConfirmationMethod.Email}
        };

        public IDictionary<string, object> ToDictionary(DynamicsGatewayOptions options, Guid debtorId)
        {
            var addressDictionary = ToDictionary(options);
            addressDictionary.Add(new KeyValuePair<string, object>("DebtorId", debtorId));
            return addressDictionary;
        }

        public virtual IDictionary<string, object> ToDictionary(DynamicsGatewayOptions options)
        {
            _ = options.PointContactRoleSave.TryGetValue(((int)PointContactRole.Value).ToString(), out var pointContactRoleId);

            var dictionary = new Dictionary<string, object>();
            dictionary.Add("Name", FullName);
            dictionary.Add("Role", pointContactRoleId);
            dictionary.Add("Communications", ContactConfirmationMethod.Value);
            dictionary.Add("TelephoneNumber", TelephoneNumber);
            dictionary.Add("EmailAddress", EmailAddress);
            return dictionary;
        }
    }
}