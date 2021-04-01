using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum PointContactRoleType
    {
        [Display(Name = "Care co-ordinator")]
        CareCoordinator = 0,

        [Display(Name = "Approved Mental Health Professional")]
        MentalHealthProfessional = 1,

        [Display(Name = "Mental health nurse")]
        MentalHealthNurse = 2
    }
}
