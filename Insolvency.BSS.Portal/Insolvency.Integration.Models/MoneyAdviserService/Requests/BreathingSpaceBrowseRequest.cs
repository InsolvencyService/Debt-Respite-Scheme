using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class BreathingSpaceBrowseRequest
    {
        public BreathingSpaceBrowseCategory Category { get; set; }

        public BreathingSpaceBrowseTask? Task { get; set; }

        public bool ShowNewestFirst { get; set; }

        public int? Page { get; set; }
    }
}
