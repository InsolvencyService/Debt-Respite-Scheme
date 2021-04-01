using System.Collections.Generic;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;

namespace Insolvency.Portal.Models.ViewModels
{
    public class BreathingSpaceBrowseViewModel
    {
        public static readonly Dictionary<BreathingSpaceBrowseTask, string> BreathingSpaceBrowseTaskDescriptions = new Dictionary<BreathingSpaceBrowseTask, string>
        {
            { BreathingSpaceBrowseTask.DebtsToBeReviewed, "Creditor has asked us to review the debt" },
            { BreathingSpaceBrowseTask.ClientsToBeReviewed, "Creditor has asked us to review the client" },
            { BreathingSpaceBrowseTask.NewDebtsProposed, "Creditor has proposed a new debt" },
            { BreathingSpaceBrowseTask.DebtsToBeTransferred, "Creditor has asked us to transfer the debt" },
            { BreathingSpaceBrowseTask.ClientsToBeTransferred, "Money Adviser has asked us to transfer the client" },
            { BreathingSpaceBrowseTask.ClientsTransferredToMoneyAdviser, "Money Adviser transferred the client to your organisation" }
        };

        public BreathingSpaceBrowseCategory BrowseByCategory { get; set; }
        public BreathingSpaceBrowseTask? BrowseByTask { get; set; }

        public MoneyAdviserLandingPageViewModel LandingPageModel { get; set; }

        public List<BreathingSpaceBrowseItem> BreathingSpaceBrowseItems { get; set; }

        public bool ShowNewestFirst { get; set; } = true;

        public int? Page { get; set; }

        public Pagination Pagination { get; set; }

        public string GetSelectedTaskDescription() =>
            BrowseByTask.HasValue ? BreathingSpaceBrowseTaskDescriptions[BrowseByTask.Value] : "";

        public bool IsTask => BrowseByCategory == BreathingSpaceBrowseCategory.TasksToDo;
        public int GetTotalItemsCount()
        {
            if (BrowseByCategory == BreathingSpaceBrowseCategory.ActiveBreathingSpaces)
            {
                return LandingPageModel.ActiveBreathingSpacesCount;
            }

            if (BrowseByCategory == BreathingSpaceBrowseCategory.SentToMoneyAdviser)
            {
                return LandingPageModel.SentToMoneyAdviserCount;
            }

            return BrowseByTask switch
            {
                BreathingSpaceBrowseTask.ClientsToBeReviewed => LandingPageModel.ClientReviewsCount,
                BreathingSpaceBrowseTask.ClientsToBeTransferred => LandingPageModel.ClientTransferRequestsCount,
                BreathingSpaceBrowseTask.ClientsTransferredToMoneyAdviser => LandingPageModel.ClientTransfersMoneyAdviserCount,
                BreathingSpaceBrowseTask.DebtsToBeReviewed => LandingPageModel.DebtReviewsCount,
                BreathingSpaceBrowseTask.DebtsToBeTransferred => LandingPageModel.SoldDebtsTransfersCount,
                BreathingSpaceBrowseTask.NewDebtsProposed => LandingPageModel.NewlyProposedDebtsCount,
                _ => 0
            };
        }
    }
}
