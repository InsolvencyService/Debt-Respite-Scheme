using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insolvency.Portal.Services.Banner
{
    public static class BannerTexts
    {
        public static Banner ClientNotRemovedAfterEligibilityReviewBanner
            => new Banner { Heading = "Confirmation", Text = "Review completed. You have not removed the client" };

        public static Banner ClientEligibilityReviewCreatedBanner
            => new Banner { Heading = "Confirmation", Text = "You’ve recorded that the creditor has asked us to review the client. We’ve added a new task to do" };

        public static Banner DebtEligibilityReviewCreatedBanner
            => new Banner { Heading = "Confirmation", Text = "You’ve recorded that the creditor has asked us to review the debt. We’ve added a new task to do" };

        public static Banner DebtRemovedAfterEligibilityReviewBanner
            => new Banner { Heading = "Confirmation", Text = "Review completed. You've removed the debt" };

        public static Banner DebtNotRemovedAfterEligibilityReviewBanner
            => new Banner { Heading = "Confirmation", Text = "Review completed. You have not removed the debt" };

        public static Banner DebtRemovedDirectlyBanner
            => new Banner { Heading = "Confirmation", Text = "Confirmation. You’ve removed the debt" };

        public static Banner DebtRemovedPresubmissionBanner
            => new Banner { Heading = "", Text = "You have deleted the debt" };

        public static Banner NewDebtAddedBanner
            => new Banner { Heading = "Confirmation", Text = "You’ve added a new debt to your client's Breathing Space" };

        public static Banner ProposedDebtAddedBanner
           => new Banner { Heading = "Confirmation", Text = "Proposed debt accepted. The debt has been added." };

        public static Banner ProposedDebtRejectedBanner
           => new Banner { Heading = "Confirmation", Text = "Proposed debt rejected. The debt has not been added." };

        public static Banner DebtorTransferCompletedBanner
           => new Banner { Heading = "Confirmation", Text = "You have removed the task from your to do list." };
    }
}
