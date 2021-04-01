using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorContactPreferenceViewModel : IConditionalRequiredValidation
    {
        private string _emailAddress;
        private string _confirmEmailAddress;

        [Required(ErrorMessage = "Please select an option")]
        public ContactPreferenceType? SubmitOption { get; set; }

        [ConditionalRequired(ErrorMessage = "Please enter email address")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [Display(Name = "Email address")]
        public string EmailAddress
        {
            get => ConditionalFlag ? _emailAddress?.Trim() : null;
            set => _emailAddress = value;
        }

        [ConditionalRequired(ErrorMessage = "Please enter confirm email address")]
        [Compare(nameof(EmailAddress), ErrorMessage = "The email addresses you entered do not match")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [Display(Name = "Confirm email address")]
        public string ConfirmEmailAddress
        {
            get => ConditionalFlag ? _confirmEmailAddress?.Trim() : null;
            set => _confirmEmailAddress = value;
        }

        public string ReturnAction { get; set; }

        public bool ConditionalFlag => SubmitOption.HasValue && SubmitOption.Value == (int)ContactPreferenceType.Email;

        public bool IsEmailChecked => string.Equals(SubmitOption?.ToString(), ContactPreferenceType.Email.ToString(), StringComparison.OrdinalIgnoreCase);
        public bool IsPostChecked => string.Equals(SubmitOption?.ToString(), ContactPreferenceType.Post.ToString(), StringComparison.OrdinalIgnoreCase);
        public bool IsNoneChecked => SubmitOption != null && !IsEmailChecked && !IsPostChecked;
    }
}
