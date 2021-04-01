using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Interfaces
{
    public interface IDebtorSearchGateway
    {
        Task<IEnumerable<AccountSearchResponse>> SearchAccountsAsync(AccountSearchBaseRequest searchParam, Guid organisationId);
    }
}
