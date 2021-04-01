using System;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorDebtEligibilityReviewConfirmViewModel : CreditorDebtEligibilityReviewViewModel
    {
        public Guid DebtId { get; set; }
        public string ReasonLabel =>
           Reason switch
           {
               DebtEligibilityReviewReasons.NotEligible => "The debt is not eligible",
               DebtEligibilityReviewReasons.UnfairPrejudices => "The Breathing Space unfairly prejudices the interests of the creditor",
               _ => null
           };
    }
}
