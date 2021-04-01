using System;
using System.Threading.Tasks;
using Insolvency.Integration.Models.CreditorService.Requests;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Interfaces;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models.ViewModels.Creditor;

namespace Insolvency.Portal.Gateways
{
    public class CreditorServiceGateway : ICreditorServiceGateway
    {
        public IApiClient Client { get; }

        public CreditorServiceGateway(IApiClient client) => Client = client;

        public async Task<BreathingSpaceResponse> GetBreathingSpace(Guid moratoriumId)
        {
            var result = await Client.GetDataAsync<BreathingSpaceResponse>($"/Creditor/BreathingSpaces/{moratoriumId}");
            return result;
        }

        public async Task SubmitClientEligibilityReview(Guid moratoriumId, CreditorClientEligibilityReviewConfirmViewModel model)
        {
            var payload = new CrDebtorEligibilityReviewRequest
            {
                CreditorId = model.CreditorId,
                CreditorNotes = model.CreditorNotes,
                EndReason = model.EndResaon.Value,
                NoLongerEligibleReason = model.NoLongerEligibleReason
            };

            await Client.CreateAsync<object, DebtorEligibilityReviewRequest>(payload, $"/Creditor/BreathingSpaces/{moratoriumId}/EligibilityReviews");
            return;
        }

        public async Task SubmitDebtEligibilityReview(CreditorDebtEligibilityReviewConfirmViewModel model)
        {
            var payload = new DebtEligibilityReviewRequest
            {
                CreditorNotes = model.CreditorNotes,
                ReviewType = model.Reason.Value
            };

            await Client.CreateAsync<object, DebtEligibilityReviewRequest>(payload, $"/Creditor/Debts/{model.DebtId}/EligibilityReviews");
            return;
        }

        public async Task SubmitDebtStoppedAllAction(CreditorDebtStoppedAllActionViewModel model)
        {
            await Client.CreateAsync<object>($"/Creditor/Debts/{model.DebtId}/ProtectionsApplied");
            return;
        }

        public async Task SubmitDebtSold(CreditorDebtSoldViewModel model)
        {
            var payload = new DebtSoldOnRequest
            {
                SoldToCreditorId = model.CreditorId
            };

            await Client.CreateAsync<object, DebtSoldOnRequest>(payload, $"/Creditor/Debts/{model.DebtId}/DebtHasBeenSold");
            return;
        }
    }
}