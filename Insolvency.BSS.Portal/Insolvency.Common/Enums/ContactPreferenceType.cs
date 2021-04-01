using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum ContactPreferenceType
    {
        [Display(Name = "Email")]
        Email = 0,

        [Display(Name = "Post")]
        Post = 1,

        [Display(Name = "None")]
        None = 2
    }
}
