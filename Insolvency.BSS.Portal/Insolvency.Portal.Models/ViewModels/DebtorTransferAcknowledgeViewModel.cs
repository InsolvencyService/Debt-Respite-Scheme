using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorTransferAcknowledgeViewModel
    {
        public DebtorTransferViewModel TransferDebtorDetails { get; set; }
        [Required(ErrorMessage = "Please select an option yes or no")]
        public bool? CompleteTransfer { get; set; }
        public Guid MoratoriumId { get; set; }

    }
}
