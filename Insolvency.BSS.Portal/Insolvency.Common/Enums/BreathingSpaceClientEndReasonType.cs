using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum BreathingSpaceClientEndReasonType
    {
        [Display(Name = "The client is not eligible because")]
        NoLongerEligible = 0,

        [Display(Name = "The client is now able to pay their debts.")]
        AbleToPayDebts = 1
    }
}