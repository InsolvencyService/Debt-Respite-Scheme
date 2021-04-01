using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{    
    public enum BreathingSpaceBrowseCategory
    {
        [Display(Name = "Active Breathing Spaces")]
        ActiveBreathingSpaces = 0,

        [Display(Name = "Tasks to do")]
        TasksToDo = 1,

        [Display(Name = "Sent to money adviser")]
        SentToMoneyAdviser = 2
    }
}
