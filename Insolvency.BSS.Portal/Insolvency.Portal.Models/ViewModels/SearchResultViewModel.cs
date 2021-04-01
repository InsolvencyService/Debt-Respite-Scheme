using System;
using Insolvency.Common;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public ClientName ClientName { get; set; }
        public string Reference { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Address Address { get; set; }
        public Guid MoratoriumId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MoratoriumStatus MoratoriumStatus { get; set; }
        public string MoratoriumType { get; set; }
        public string OrganisationName { get; set; }
        public DateTime CreatedOn { get; set; }

        public string FormatedDob => DateOfBirth.ToString(Constants.UkDateFullMonthFormat);
        public string FormattedStartDate => StartDate.HasValue ? StartDate?.ToString(Constants.UkDateFullMonthFormat) : null;
        public string FormattedEndDate => EndDate.HasValue ? EndDate?.ToString(Constants.UkDateFullMonthFormat) : null;
        public bool IsActive => MoratoriumStatus == MoratoriumStatus.Active;
        public bool IsEnded => MoratoriumStatus == MoratoriumStatus.Cancelled || MoratoriumStatus == MoratoriumStatus.Expired;
        public bool IsDraft => MoratoriumStatus == MoratoriumStatus.Draft;
        public bool HasDob => DateOfBirth != default;
        public bool HasOrganisationName => !string.IsNullOrEmpty(OrganisationName);

        public string FormattedMoratoriumStatus
        {
            get
            {
                switch (MoratoriumStatus)
                {
                    case MoratoriumStatus.Draft:
                        return "Inactive";
                    case MoratoriumStatus.Cancelled:
                    case MoratoriumStatus.Expired:
                        return "Ended";
                    case MoratoriumStatus.Active:
                        return "Active";
                }

                return MoratoriumStatus.ToString();
            }
        }

        public string EndedDateHelperText
        {
            get
            {
                if (IsEnded)
                    return "Ended:";
                
                if (IsActive)
                    return "Due to end:";

                return string.Empty;
            }
        }
    }
}
