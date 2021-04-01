using System;
using System.ComponentModel.DataAnnotations;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorRadioYesNoViewModel
    {
        public Guid MoratoriumId { get; set; }

        [Required(ErrorMessage = "Please select an option yes or no")]
        public virtual string SubmitOption { get; set; }

        public bool IsRadioInline { get; set; }

        public bool SubmitNow => SubmitOption == Option.Yes.ToString();

        public string YesHint { get; set; }
        public string NoHint { get; set; }

        public bool IsYesChecked { get; set; }
        public bool IsNoChecked { get; set; }

        public bool HasYesHint => !string.IsNullOrEmpty(YesHint);
        public bool HasNoHint => !string.IsNullOrEmpty(NoHint);
    }
}
