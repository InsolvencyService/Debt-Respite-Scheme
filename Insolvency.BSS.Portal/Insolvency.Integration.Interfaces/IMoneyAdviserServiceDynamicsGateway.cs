using System;
using System.Threading.Tasks;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.BreathingSpaceCreation;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Interfaces
{
    public interface IMoneyAdviserServiceDynamicsGateway
    {
        Task<MoneyAdviserLandingPageStatsResponse> GetMoneyAdviserLandingPageStats(Guid organisationId);
        Task<Guid> CreateClientWithDetailsAsync(ClientDetailsCreateRequest model, Guid organisationId);
        Task UpdateClientWithDetailsAsync(ClientDetailsUpdateRequest model);
        Task<Guid> AddClientPreviousNameAsync(ClientPreviousNameCreateRequest model);
        Task<Guid> UpdateClientPreviousNameAsync(ClientPreviousNameUpdateRequest model);
        Task<Guid> CreateDebtorAddressAsync(CustomAddress model);
        Task UpdateAddressAsync(UpdateCustomAddress model);
        Task<DebtorAccountSaveResponse> SaveDebtorAsync(DebtorAccountSaveRequest model);
        Task<Guid> CreateDebtAsync(CreateDebtRequest model);
        Task<Guid> UpdateDebtAsync(UpdateDebtRequest model);
        Task<Guid> SetBreathingSpaceAsMentalHealth(Guid moratoriumId);
        Task<Guid> SetBreathingSpaceAsStandard(Guid moratoriumId);
        Task<Guid> DebtorHideAddressAsync(DebtorHideAddressRequest model);
        Task<BusinessAdressResponse> AddDebtorBusinessAddressAsync(BusinessAddressRequest model);
        Task UpdateBusinessNameAsync(BusinessNameUpdateRequest model);
        Task<NominatedContactCreateResponse> CreateNominatedContactAsync(NominatedContactCreateRequest model);
        Task UpdateNominatedContactAsync(NominatedContactUpdateRequest model);
        Task DebtorSetContactPreference(DebtorContactPreferenceRequest model);
        Task<DebtorNominatedContactResponse> GetNominatedContactAsync(Guid moratoriumId);
        Task<Guid> DebtorEndAccount(DebtorAccountEndRequest model);
        Task<BSCreationResponse> CreateBreathingSpace(CreateBreathingSpace moratorium, Guid organisationId);
        Task<DebtDetailResponse> GetDebtDetail(Guid debtId);
        Task<Guid> SubmitDebtEligibilityReview(DebtEligibilityReviewResponseRequest model);
        Task<Guid> DebtorReviewClientEligibility(DebtorEligibilityReviewResponseRequest model);
        Task ConfirmDebtSold(Guid debtId);
        Task RemoveDebt(RemoveDebtRequest removeDebtRequest);
        Task RemoveDebtPresubmission(Guid debtId, Guid organisationId);
        Task MakeProposedDebtDetermination(ReviewProposedDebtRequest reviewProposedDebtRequest);
        Task CreateDebtAndAdHocCreditor(CreateAdHocDebtRequest createAdHocDebtRequest);
        Task TransferDebtor(TransferDebtorRequest transferDebtorRequest);
        Task CompleteTransferDebtor(TransferDebtorCompleteRequest transferDebtorCompleteRequest);
        Task AcknowledgeDebtorTransfer(DebtorTransferRequest debtorTransferRequest);
    }
}
