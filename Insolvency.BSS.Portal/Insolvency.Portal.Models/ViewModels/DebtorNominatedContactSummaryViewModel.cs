using System;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorNominatedContactSummaryViewModel
    {
        public DebtorNominatedContactSummaryViewModel() { }
        public DebtorNominatedContactSummaryViewModel(DebtorNominatedContactResponse nominatedContact)
        {
            if (nominatedContact is null)
                throw new ArgumentNullException(nameof(nominatedContact));

            ContactId = nominatedContact.ContactId;
            RoleId = nominatedContact.RoleId;
            PointContactRole = nominatedContact.PointContactRole;
            FullName = nominatedContact.FullName;
            TelephoneNumber = nominatedContact.TelephoneNumber;
            EmailAddress = nominatedContact.EmailAddress;
            NotificationMethod = nominatedContact.NotificationMethod;
            CommunicationAddress = nominatedContact.CommunicationAddress != null ? new Address
            {
                AddressId = nominatedContact.CommunicationAddress.AddressId,
                AddressLine1 = nominatedContact.CommunicationAddress.AddressLine1,
                AddressLine2 = nominatedContact.CommunicationAddress.AddressLine2,
                TownCity = nominatedContact.CommunicationAddress.TownCity,
                County = nominatedContact.CommunicationAddress.County,
                Country = nominatedContact.CommunicationAddress.Country,
                Postcode = nominatedContact.CommunicationAddress.Postcode,
            } : null;
        }

        public string FullName { get; set; }
        public PointContactRoleType PointContactRole { get; set; }
        public string TelephoneNumber { get; set; }
        public PointContactConfirmationMethod NotificationMethod { get; set; }
        public string EmailAddress { get; set; }
        public Address CommunicationAddress { get; set; }

        public string FormattedPointContactRole => PointContactRole.GetDisplayName();
        public string FormattedNotificationMethod => NotificationMethod.GetDisplayName();

        public bool AllowAddTelephoneNumber => string.IsNullOrEmpty(TelephoneNumber);
        public bool AllowAddEmailAddress => string.IsNullOrEmpty(EmailAddress);
        public bool DisplayAddress => NotificationMethod == PointContactConfirmationMethod.Post && CommunicationAddress != null;

        public Guid ContactId { get; set; }
        public Guid RoleId { get; set; }

        public DebtorNominatedContactViewModel ToNominatedContactViewModel()
        {
            return new DebtorNominatedContactViewModel
            {
                ContactId = ContactId,
                RoleId = RoleId,
                PointContactRole = PointContactRole.ToString(),
                FullName = FullName,
                TelephoneNumber = TelephoneNumber,
                EmailAddress = EmailAddress,
                ConfirmEmailAddress = EmailAddress,
                ContactConfirmationMethod = NotificationMethod.ToString(),
                CommunicationAddress = CommunicationAddress
            };
        }
    }
}
