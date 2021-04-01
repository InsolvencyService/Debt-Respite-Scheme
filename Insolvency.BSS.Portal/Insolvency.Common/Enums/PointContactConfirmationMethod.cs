using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum PointContactConfirmationMethod
    {
        [Display(Name = "Email")]
        Email = 0,

        [Display(Name = "Post")]
        Post = 1
    }
}
