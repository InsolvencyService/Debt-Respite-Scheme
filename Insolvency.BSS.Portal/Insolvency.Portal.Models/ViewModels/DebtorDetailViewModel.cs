using System.Collections.Generic;
using System.Linq;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorDetailViewModel
    {
        public DebtorDetailViewModel()
        {
        }

        public DebtorDetailViewModel(BreathingSpaceResponse accountSummary)
        {
            PersonalDetail = new DebtorPersonalDetailViewModel(accountSummary.DebtorDetails);
            AddressDetail = new DebtorAddressViewModel(accountSummary);
            NotificationDetail = new DebtorNotificationPartialViewModel(accountSummary.DebtorDetails);
            BusinessDetails = accountSummary.DebtorBusinessDetails
                            ?.Select(b =>
                                new BusinessAddressViewModel(
                                    b.BusinessName,
                                    b.Address,
                                    accountSummary.CurrentAddress,
                                    accountSummary.DebtorDetails.AddressHidden,
                                    b.Id
                                ))
                            ?? Enumerable.Empty<BusinessAddressViewModel>();
            DebtorNominatedContactSummary = accountSummary.DebtorNominatedContactResponse != null
                ? new DebtorNominatedContactSummaryViewModel(accountSummary.DebtorNominatedContactResponse)
                : null;
        }

        public DebtorPersonalDetailViewModel PersonalDetail { get; set; }
        public DebtorAddressViewModel AddressDetail { get; set; }
        public DebtorNotificationPartialViewModel NotificationDetail { get; set; }
        public IEnumerable<BusinessAddressViewModel> BusinessDetails { get; set; }
        public DebtorNominatedContactSummaryViewModel DebtorNominatedContactSummary { get; set; }
        public bool HasBusiness => BusinessDetails.Any();
    }
}
