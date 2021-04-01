using System;
using System.Threading.Tasks;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Portal.Models.ViewModels.Creditor;

namespace Insolvency.Portal.Interfaces
{
    public interface ICreditorServiceGateway
    {
        Task<BreathingSpaceResponse> GetBreathingSpace(Guid moratoriumId);
        Task SubmitClientEligibilityReview(Guid moratoriumId, CreditorClientEligibilityReviewConfirmViewModel model);
        Task SubmitDebtEligibilityReview(CreditorDebtEligibilityReviewConfirmViewModel model);
        Task SubmitDebtStoppedAllAction(CreditorDebtStoppedAllActionViewModel model);
        Task SubmitDebtSold(CreditorDebtSoldViewModel model);
    }
}
