using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorNotificationPartialViewModel
    {
        public DebtorNotificationPartialViewModel()
        {
        }

        public DebtorNotificationPartialViewModel(DebtorDetailsResponse model)
        {
            PreferenceType = model.ContactPreference;
            PreferenceTypeLabel = model.ContactPreferenceLabel;
            EmailAddress = model.NotificationEmailAddress;
        }

        public ContactPreferenceType? PreferenceType { get; set; }
        public string PreferenceTypeLabel { get; set; }
        public string EmailAddress { get; set; }
    }
}
