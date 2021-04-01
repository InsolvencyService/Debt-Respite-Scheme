using System;
using System.Threading.Tasks;
using Insolvency.Integration.Models.CreditorService.Requests;
using Insolvency.Integration.Models.CreditorService.Responses;

namespace Insolvency.Integration.Interfaces
{
    public interface ICreditorServiceDynamicsGateway
    {
        Task DebtStopAllAction(Guid debtId, Guid creditorId);
        Task<ProposeDebtResponse> ProposeADebt(ProposeDebtRequest model);
    }
}
