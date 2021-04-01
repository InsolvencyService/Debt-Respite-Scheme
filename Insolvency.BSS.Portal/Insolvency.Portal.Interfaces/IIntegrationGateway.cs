using System;
using System.Threading.Tasks;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Portal.Models;
using Insolvency.Portal.Models.ViewModels;

namespace Insolvency.Portal.Interfaces
{
    public interface IIntegrationGateway
    {
        Task<Guid> CreateClientAsync(ClientDetailsCreateViewModel model);
        Task<Guid> AddClientPreviousNameAsync(ClientAddPreviousNameViewModel model, Guid moratoriumId);
        Task<Guid> UpdateClientPreviousNameAsync(ClientAddPreviousNameViewModel model, Guid moratoriumId);
        Task<ClientNamesSummaryViewModel> GetClientNamesAsync(Guid moratoriumId);
        Task UpdateClientAsync(ClientDetailsCreateViewModel model);
        Task<Guid> AddAdHocCreditorAddressAsync(Address creditorAddress, Guid creditorId);
        Task<Guid> CreateDebtorAddressAsync(Address debtorAddress, Guid moratoriumId);
        Task<Guid> CreateDebtorPreviousAddressAsync(Address debtorAddress, Guid moratoriumId);
        Task<AccountSearchViewModel> SearchAccounts(AccountSearchViewModel searchViewModel);
        Task<CreditorResponse> GetGenericCreditorByIdAsync(string id);
        Task<Guid> CreatDebtAsync(DebtViewModel model);
        Task<Guid> UpdateDebtAsync(DebtViewModel model);
        Task<DebtorAccountConfirmationViewModel> SubmitDebtorAccount(DebtorRadioYesNoViewModel model);
        Task<CreditorSearchResultsViewModel> CmpCreditorSearch(string creditorName);
        Task<CreditorSearchResponse> CmpCreditorSearchAjax(string creditorName);
        Task<DebtorDetailViewModel> GetDebtorConfirmDetails(Guid moratoriumId);
        Task SetBreathingSpaceAsMentalHealth(Guid moratoriumId);
        Task SetBreathingSpaceAsStandard(Guid moratoriumId);
        Task<DebtorAccountSummaryViewModel> GetAccountSummary(Guid moratoriumId);
        Task<MoneyAdviserLandingPageViewModel> GetMoneyAdviserLandingPageStats();
        Task BrowseBreathingSpacesAsync(BreathingSpaceBrowseViewModel model);
        Task<DebtorAddressViewModel> GetDebtorAddresses(Guid moratoriumId);
        Task<Guid> DebtorHideAddressAsync(DebtorRadioYesNoViewModel model);
        Task<BusinessAdressResponse> DebtorAddBusinessAsync(DebtorAddBusinessViewModel model);
        Task DebtorUpdateBusinessAsync(DebtorAddBusinessViewModel businessAddress);
        Task<NominatedContactCreateResponse> CreateNominatedContactAsync(DebtorNominatedContactViewModel model);
        Task UpdateNominatedContactAsync(DebtorNominatedContactViewModel viewModel);
        Task<Guid> CreateAdHocCreditor(string name);
        Task DebtorSetContactPreference(Guid moratoriumId, DebtorContactPreferenceViewModel model);
        Task DebtorEndAccount(Guid moratoriumId, DebtorAccountEndReasonConfirmationViewModel model);
        Task<Guid> DebtorReviewClientEligibility(Guid moratoriumId, DebtorEligibilityReviewDecisionViewModel model);
        Task<DebtorNominatedContactSummaryViewModel> GetDebtorNominatedContactSummary(Guid moratoriumId);
        Task<DebtDetailViewModel> GetDebtDetail(Guid debtId);
        Task<Guid> SubmitDebtEligibilityReview(DebtEligibilityReviewSummaryViewModel viewModel);
        Task<Guid> SubmitDebtEligibilityReviewTask(DebtElgibilityReviewTaskSummaryViewModel model);
        Task<Guid> SubmitDebtorEligibilityReviewTask(DebtorEligibilityReviewSummaryViewModel model);
        Task SubmitDebtSold(DebtSoldOnRequest model);
        Task ConfirmDebtSold(Guid debtId);
        Task<bool> RemoveDebt(RemoveDebtRequest removeDebtRequest);
        Task<bool> RemoveDebtPresubmission(Guid debtId);
        Task MakeProposedDebtDetermination(ReviewProposedDebtRequest reviewProposedDebtRequest);
        Task TransferDebtor(TransferDebtorRequest transferDebtorRequest);
        Task CompleteTransferDebtor(Guid moratoriumIdId);
        Task AcknowledgeDebtorTransfer(DebtorTransferAcknowledgeViewModel model);
    }
}
