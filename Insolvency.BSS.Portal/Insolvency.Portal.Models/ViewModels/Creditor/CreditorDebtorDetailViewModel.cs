using System.Collections.Generic;
using System.Linq;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorDebtorDetailViewModel
    {
        public CreditorDebtorDetailViewModel(BreathingSpaceResponse debtorDetail)
        {
            PersonalDetail = new DebtorPersonalDetailViewModel(debtorDetail.DebtorDetails);
            AddressDetail = new DebtorAddressViewModel(debtorDetail);
            NotificationDetail = new DebtorNotificationPartialViewModel(debtorDetail.DebtorDetails);
            BusinessDetails = debtorDetail.DebtorBusinessDetails
                            ?.Select(b =>
                                new BusinessAddressViewModel(
                                    b.BusinessName,
                                    b.Address,
                                    debtorDetail.CurrentAddress,
                                    debtorDetail.DebtorDetails.AddressHidden,
                                    b.Id
                                ))
                            ?? Enumerable.Empty<BusinessAddressViewModel>();
        }

        public DebtorPersonalDetailViewModel PersonalDetail { get; set; }
        public DebtorAddressViewModel AddressDetail { get; set; }
        public DebtorNotificationPartialViewModel NotificationDetail { get; set; }
        public IEnumerable<BusinessAddressViewModel> BusinessDetails { get; set; }

    }
}
