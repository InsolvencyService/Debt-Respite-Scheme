using System;
using Insolvency.Common;

namespace Insolvency.Integration.Models
{
    public class BreathingSpaceBrowseItem
    {
        public Guid BreathingSpaceId { get; set; }
        public string BreathingSpaceReference { get; set; }
        public DateTimeOffset? DateStarted { get; set; }
        public DateTimeOffset? DateEnded { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FormattedActiveMoratoriumStartDate => DateStarted?.ToString(Constants.PrettyDateFormat);

        public string FormattedActiveMoratoriumEndDate => DateEnded?.ToString(Constants.PrettyDateFormat);

        public bool HasStartDate => DateStarted.HasValue;

        public bool HasEndDate => DateEnded.HasValue;
    }
}
