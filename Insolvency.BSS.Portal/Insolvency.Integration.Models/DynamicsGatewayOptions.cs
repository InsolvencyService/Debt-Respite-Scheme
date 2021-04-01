using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models
{
    public class DynamicsGatewayOptions
    {
        public Guid GovermentCreditorId { get; set; }
        public Guid MentalHealthMoratoriumTypeId { get; set; }
        public Dictionary<string, int> PointContactRoleSave { get; set; }
        public Dictionary<string, int> PointContactRoleRead { get; set; }
        public Dictionary<string, int> ContactPreferenceReadMap { get; set; }
        public Dictionary<string, string> BreathingSpaceEndReason { get; set; }
        public Dictionary<string, string> BreathingSpaceEndReasonNoLongerEligibleReason { get; set; }
        public Dictionary<string, string> EligibilityReviewParentReason { get; set; }
        public Dictionary<string, string> EligibilityReviewChildReason { get; set; }
        public Dictionary<string, string> DebtEligibilityReviewReasons { get; set; }
        public Dictionary<DebtRemovalReason, string> DebtRemovalReason { get; set; }
        public Dictionary<string, DebtEligibilityReviewStatus> DebtEligibilityReviewStatus { get; set; }
        public Dictionary<string, DebtStatus> DebtStatus { get; set; }
        public Dictionary<string, DebtorEligibilityReviewStatus> DebtorEligibilityReviewStatus { get; set; }
        public Dictionary<DebtorEligibilityReviewStatus, string> DebtorEligibilityReviewStatusCode { get; set; }

        public Guid GetEligibilityReviewParentReasonId(BreathingSpaceClientEndReasonType enumValue)
        {
            var enumNumber = ((int)enumValue).ToString();
            var result = EligibilityReviewParentReason[enumNumber];
            return new Guid(result);
        }

        public Guid DebtorEligibilityReviewStatusId(DebtorEligibilityReviewStatus status)
        {
            return Guid.Parse(DebtorEligibilityReviewStatus.First(m => m.Value == status).Key);
        }

        public Guid DebtEligibilityReviewStatusId(DebtEligibilityReviewStatus status)
        {
            return Guid.Parse(DebtEligibilityReviewStatus.First(m => m.Value == status).Key);
        }

        public Guid? GetEligibilityReviewChildReasonId(BreathingSpaceEndReasonNoLongerEligibleReasonType? enumValue)
        {
            if (enumValue is null) return null;

            var enumNumber = ((int)enumValue).ToString();
            var result = EligibilityReviewChildReason[enumNumber];
            return new Guid(result);
        }

        public Guid GetDebtEligibilityReviewReasonId(DebtEligibilityReviewReasons enumValue)
        {
            var enumNumber = ((int)enumValue).ToString();
            var result = DebtEligibilityReviewReasons[enumNumber];
            return new Guid(result);
        }        

        public BreathingSpaceClientEndReasonType? GetEligibilityReviewParentReasonById(Guid? reasonId)
        {
            if (reasonId is null) return null;

            var result = GetKeyByValue(EligibilityReviewParentReason, reasonId.ToString());

            var reason = int.Parse(result);
            return (BreathingSpaceClientEndReasonType)reason;
        }

        public BreathingSpaceEndReasonNoLongerEligibleReasonType? GetEligibilityReviewChildReasonById(Guid? reasonId)
        {
            if (reasonId is null) return null;

            var result = GetKeyByValue(EligibilityReviewChildReason, reasonId.ToString());

            var reason = int.Parse(result);
            return (BreathingSpaceEndReasonNoLongerEligibleReasonType)reason;
        }

        private string GetKeyByValue(Dictionary<string, string> dictionary, string value) => dictionary.FirstOrDefault(pair => pair.Value == value).Key;

    }
}
