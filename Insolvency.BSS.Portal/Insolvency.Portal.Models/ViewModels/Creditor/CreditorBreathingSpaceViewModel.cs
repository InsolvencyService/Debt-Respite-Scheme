using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorBreathingSpaceViewModel
    {
        public CreditorBreathingSpaceViewModel(BreathingSpaceResponse breathingSpace)
        {
            if (breathingSpace is null)
                throw new ArgumentNullException(nameof(breathingSpace));

            DebtorDetail = new CreditorDebtorDetailViewModel(breathingSpace);

            Debts = breathingSpace.DebtDetails
                .Select(debtDetail => new CreditorDebtPartialViewModel(
                    debtDetail,
                    DebtorDetail.PersonalDetail.ActiveMoratoriumEndDate,
                    DebtorDetail.PersonalDetail.IsInMentalHealthMoratorium))
                .ToList();

            EligibilityReviews = breathingSpace.DebtorEligibilityReviews.ToList();
        }

        public CreditorDebtorDetailViewModel DebtorDetail { get; set; }
        public List<CreditorDebtPartialViewModel> Debts { get; set; }
        public List<DebtorEligibilityReviewResponse> EligibilityReviews { get; set; }

        public string BannerHeading { get; set; }
        public string BannerText { get; set; }
    }
}
