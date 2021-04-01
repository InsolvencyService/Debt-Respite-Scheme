
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorClientEligibilityReviewConfirmViewModel : CreditorClientEligibilityReviewViewModel
    {
        public string ReasonLabel =>
             this switch
             {
                 {
                     EndResaon: BreathingSpaceClientEndReasonType.NoLongerEligible,
                     NoLongerEligibleReason: BreathingSpaceEndReasonNoLongerEligibleReasonType.NotInEnglandOrWales
                 } => "The client is not eligible because they do not live or usually reside in England or Wales",
                 {
                     EndResaon: BreathingSpaceClientEndReasonType.NoLongerEligible,
                     NoLongerEligibleReason: BreathingSpaceEndReasonNoLongerEligibleReasonType.DebtReliefOrder
                 } => "The client is not eligible because they have a Debt relief Order",
                 {
                     EndResaon: BreathingSpaceClientEndReasonType.NoLongerEligible,
                     NoLongerEligibleReason: BreathingSpaceEndReasonNoLongerEligibleReasonType.InterimOrderOrIndividualVoluntaryArrangement
                 } => "The client is not eligible because they have an interim order or Individual Voluntary Arrangement",
                 {
                     EndResaon: BreathingSpaceClientEndReasonType.NoLongerEligible,
                     NoLongerEligibleReason: BreathingSpaceEndReasonNoLongerEligibleReasonType.UndischargedBankrupt
                 } => "The client is not eligible because they are an undischarged bankrupt",
                 {
                     EndResaon: BreathingSpaceClientEndReasonType.NoLongerEligible,
                     NoLongerEligibleReason: BreathingSpaceEndReasonNoLongerEligibleReasonType.HasHadAnotherBreathingSpaceWithinTwelveMonths
                 } => "The client is not eligible because they have had another Breathing Space in the last 12 months",
                 {
                     EndResaon: BreathingSpaceClientEndReasonType.AbleToPayDebts,
                 } => "The client has sufficient funds to discharge their debt",
                 _ => null
             };
    }
}
