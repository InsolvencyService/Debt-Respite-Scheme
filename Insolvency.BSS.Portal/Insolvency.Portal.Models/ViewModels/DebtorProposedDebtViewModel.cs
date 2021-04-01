using System.ComponentModel.DataAnnotations;
using Insolvency.Common.Attributes;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorProposedDebtViewModel : IConditionalRequiredValidation
    {
        public DebtDetailViewModel DebtDetailViewModel { get; set; }

        [Required(ErrorMessage = "Please provide an outcome for the proposed debt")]
        public bool? AcceptProposedDebt { get; set; }

        [ConditionalRequired(ErrorMessage = "Please provide further details")]
        public string RemovalReason { get; set; }

        public bool ConditionalFlag => AcceptProposedDebt.HasValue && !AcceptProposedDebt.Value;
    }
}
