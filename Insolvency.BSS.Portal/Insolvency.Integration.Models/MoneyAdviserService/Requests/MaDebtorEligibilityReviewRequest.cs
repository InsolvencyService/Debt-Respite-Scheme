using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Integration.Models.Shared.Requests;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class MaDebtorEligibilityReviewRequest : DebtorEligibilityReviewRequest
    {
        [Required]
        public override Guid? CreditorId { get; set; }
    }
}
