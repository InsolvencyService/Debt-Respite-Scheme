using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum BreathingSpaceEndReasonNoLongerEligibleReasonType
    {
        [Display(Name = " they do not live or usually reside in England or Wales")]
        NotInEnglandOrWales = 0,
        [Display(Name = " they have a Debt relief Order")]
        DebtReliefOrder = 1,
        [Display(Name = " they have an interim order or Individual Voluntary Arrangement")]
        InterimOrderOrIndividualVoluntaryArrangement = 2,
        [Display(Name = " they are an undischarged bankrupt")]
        UndischargedBankrupt = 3,
        [Display(Name = " they have had another Breathing Space in the last 12 months")]
        HasHadAnotherBreathingSpaceWithinTwelveMonths = 4
    }
}