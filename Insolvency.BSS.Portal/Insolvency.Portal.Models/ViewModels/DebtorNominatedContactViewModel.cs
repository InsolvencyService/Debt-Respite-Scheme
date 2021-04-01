using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Common;
using Insolvency.Common.Attributes;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorNominatedContactViewModel : IConditionalRequiredValidation
    {
        private string _emailAddress;
        private string _confirmEmailAddress;

        public DebtorNominatedContactViewModel() { }

        [Required(ErrorMessage = "Please select a point of contacts role")]
        [Display(Name = "Point of contacts role")]
        public string PointContactRole { get; set; }

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

        [ConditionalRequired(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [Display(Name = "Email address")]
        public string EmailAddress
        {
            get => _emailAddress?.Trim();
            set => _emailAddress = value;
        }

        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [ConditionalRequired(ErrorMessage = "Please enter confirm email address")]
        [MaxLength(256, ErrorMessage = "There is a problem Invalid email length")]
        [Compare(nameof(EmailAddress), ErrorMessage = "The email addresses you entered do not match")]
        [Display(Name = "Confirm email address")]
        public string ConfirmEmailAddress
        {
            get => _confirmEmailAddress?.Trim();
            set => _confirmEmailAddress = value;
        }

        [Required(ErrorMessage = "Please select a confirmation method")]
        [Display(Name = "Contact confirmation method")]
        public string ContactConfirmationMethod { get; set; }

        public bool ConditionalFlag => IsEmailChecked;

        public Guid MoratoriumId { get; set; }

        public Address CommunicationAddress { get; set; }

        public string InputFocus { get; set; }
        public string ReturnAction { get; set; }
        public bool IsAddressChangeRequested { get; set; }
        public bool IsEdit { get; set; }

        public bool AllowAddressChange => (IsPostChecked && IsEdit && IsAddressChangeRequested)
                                       || (IsPostChecked && !IsEdit)
                                       || (IsAddressChangeRequested && IsPostChecked)
                                       || (IsPostChecked && CommunicationAddress?.Postcode != null);

        public bool IsFullNameFocus => string.Equals(InputFocus, nameof(FullName), StringComparison.OrdinalIgnoreCase);
        public bool IsEmailAddressFocus => string.Equals(InputFocus, nameof(EmailAddress), StringComparison.OrdinalIgnoreCase);
        public bool IsTelephoneNumber => string.Equals(InputFocus, nameof(TelephoneNumber), StringComparison.OrdinalIgnoreCase);
        public bool IsCareCordinatorChecked => string.Equals(PointContactRole, PointContactRoleType.CareCoordinator.ToString(), StringComparison.OrdinalIgnoreCase);
        public bool IsMentalHealthNurseChecked => string.Equals(PointContactRole, PointContactRoleType.MentalHealthNurse.ToString(), StringComparison.OrdinalIgnoreCase);
        public bool IsMentalHealthProfessionalChecked => string.Equals(PointContactRole, PointContactRoleType.MentalHealthProfessional.ToString(), StringComparison.OrdinalIgnoreCase);
        public bool IsEmailChecked => string.Equals(ContactConfirmationMethod, PointContactConfirmationMethod.Email.ToString(), StringComparison.OrdinalIgnoreCase);
        public bool IsPostChecked => string.Equals(ContactConfirmationMethod, PointContactConfirmationMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase);

        public Guid ContactId { get; set; }
        public Guid RoleId { get; set; }

        public NominatedContactCreateRequest ToCreateNominatedContactRequestModel()
        {
            return new NominatedContactCreateRequest
            {
                FullName = FullName,
                PointContactRole = Enum.Parse<PointContactRoleType>(PointContactRole),
                TelephoneNumber = TelephoneNumber,
                EmailAddress = EmailAddress,
                ConfirmEmailAddress = ConfirmEmailAddress,
                ContactConfirmationMethod = Enum.Parse<PointContactConfirmationMethod>(ContactConfirmationMethod),
                MoratoriumId = MoratoriumId,
                Address = CommunicationAddress is null ? null : new NominatedContactAddress(CommunicationAddress?.ToCustomAddress())
            };
        }

        public NominatedContactUpdateRequest ToUpdateNominatedContactRequestModel()
        {
            return new NominatedContactUpdateRequest
            {
                ContactId = ContactId,
                FullName = FullName,
                PointContactRole = Enum.Parse<PointContactRoleType>(PointContactRole),
                TelephoneNumber = TelephoneNumber,
                EmailAddress = EmailAddress,
                ConfirmEmailAddress = ConfirmEmailAddress,
                ContactConfirmationMethod = Enum.Parse<PointContactConfirmationMethod>(ContactConfirmationMethod),
                Address = CommunicationAddress is null ? null : new NominatedContactAddress(CommunicationAddress?.ToCustomAddress())
            };
        }
    }
}
