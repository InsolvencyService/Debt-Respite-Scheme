using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Interfaces
{
    public interface ICommonDynamicsGateway
    {
        Task DebtSoldOn(DebtSoldOnRequest model);
        Task CreateDebtorEligibilityReviewRequest(DebtorEligibilityReviewRequest model);
        Task CreateDebtEligibilityReviewRequest(DebtEligibilityReviewRequest model);
        Task<IEnumerable<CreditorSearchDetailedResponse>> GetAllCMPCreditors();
        Task<DebtorDetailsResponse> GetClientNamesAsync(Guid moratoriumId);
        Task<BreathingSpaceResponse> GetMaBreathingSpaceAsync(Guid moratoriumId, Guid organisationId);
        Task<BreathingSpaceResponse> GetCreditorBreathingSpaceAsync(Guid moratoriumId, Guid organisationId);
        Task<Guid> CreateAdHocCreditor(string name);
        Task<Guid> AddAdHocCreditorAddressAsync(CustomAddress model);
        Task<CreditorSearchResponse> SearchCmpCreditors(string creditorName);
        Task<Creditor> GetGenericCreditorById(Guid creditorId);
        Task TransferDebtToNewAdhocCreditor(DebtSoldOnToAdHocCreditorRequest model);
        Task<bool> ExpireMoratoriumAsync();
    }
}
