using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorBreathingSpaceTypeViewModel
    {
        [Required(ErrorMessage = "Please select an option")]
        public string SubmitOption { get; set; }
    }
}
