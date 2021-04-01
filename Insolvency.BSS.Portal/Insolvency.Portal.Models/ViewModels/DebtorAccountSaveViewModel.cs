using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAccountSaveViewModel
    {
        public enum Option
        {
            Yes = 1,
            No = 2
        }

        [Required(ErrorMessage = "Please select an option (Yes) / (No)")]
        public string SubmitOption { get; set; }

        public bool SubmitNow => SubmitOption == Option.Yes.ToString();
    }
}
