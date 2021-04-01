using System.ComponentModel.DataAnnotations;

namespace Insolvency.Common.Enums
{
    public enum BreathingSpaceEndReasonType
    {
        [Display(Name = "The client has stopped receiving mental health crisis treatment")]
        StoppedTreatment = 0,

        [Display(Name = "We've not been able to reach the point of contact to confirm mental health crisis treatment is ongoing")]
        UnableToReachPointOfContact = 1,

        [Display(Name = "The client is not eligible anymore because")]
        NoLongerEligible = 2,

        [Display(Name = "The client is now using a debt management solution")]
        DebtManagementSolution = 3,

        [Display(Name = "The client is now able to pay their debts")]
        AbleToPayDebts = 4,

        [Display(Name = "The client has asked us to cancel")]
        Cancelled = 5,

        [Display(Name = "The client has died")]
        Deceased = 6,

        [Display(Name = "We consider the information provided by the approved mental health professional contains inaccurate, misleading or fraudulent information")]
        InvalidInformation = 7,

        [Display(Name = "The client has not met their Breathing Space obligations")]
        NotCompliedWithObligations = 8,

        [Display(Name = "We have not been able to consult the client")]
        UnableToContactClient = 9
    }
}