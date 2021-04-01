using System;
using System.Threading.Tasks;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Interfaces
{
    public interface IBreathingSpaceBrowseGateway
    {
        Task<BreathingSpaceBrowseResponse> BrowserBreathingSpaceByAsync(BreathingSpaceBrowseCategory category, BreathingSpaceBrowseTask? task, Guid moneyAdviserId, bool showNewestFirst);
    }
}
