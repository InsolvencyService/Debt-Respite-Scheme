using System.ComponentModel.DataAnnotations;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorRemoveDebtViewModel
    {
        public DebtDetailViewModel DebtDetailViewModel { get; set; }

        [Required(ErrorMessage = "Please select a reason for removing this debt")]
        public DebtRemovalReason? Reason { get; set; }

        [Required(ErrorMessage = "Please provide more details for removing this debt")]
        public string MoreDetails { get; set; }

     
    }
}
