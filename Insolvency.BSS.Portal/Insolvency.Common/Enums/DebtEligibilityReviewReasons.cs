using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum DebtEligibilityReviewReasons
    {
        [Display(Name = "The debt is not eligible")]
        NotEligible = 0,
        [Display(Name = "The Breathing Space unfairly prejudices the interests of the creditor")]
        UnfairPrejudices = 1
    }
}