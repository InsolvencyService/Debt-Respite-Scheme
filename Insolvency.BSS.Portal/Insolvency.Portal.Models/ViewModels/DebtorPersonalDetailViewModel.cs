using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorPersonalDetailViewModel
    {
        public DebtorPersonalDetailViewModel() { }
        public DebtorPersonalDetailViewModel(DebtorDetailsResponse debtorDetails)
        {
            FirstName = debtorDetails.FirstName;
            MiddleName = debtorDetails.MiddleName;
            Surname = debtorDetails.LastName;
            PreviousNames = debtorDetails.PreviousNames?.Select(x => new ClientName
            {
                NameId = x.Id,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName
            });
            DateOfBirth = debtorDetails?.DateOfBirth ?? default;
            IsInMentalHealthMoratorium = debtorDetails.IsInMentalHealthMoratorium ?? false;
            ActiveMoratoriumEndDate = debtorDetails.MoratoriumStatus == Insolvency.Common.Enums.MoratoriumStatus.Cancelled ? debtorDetails.CancellationDate : debtorDetails.EndsOn;
            ActiveMoratoriumStartDate = debtorDetails.StartsOn;
            MoratoriumStatus = SetMoratoriumStatus(debtorDetails.MoratoriumStatus);
            IsActive = debtorDetails.MoratoriumStatus == Insolvency.Common.Enums.MoratoriumStatus.Active;
            IsEnded = debtorDetails.MoratoriumStatus == Insolvency.Common.Enums.MoratoriumStatus.Cancelled || debtorDetails.MoratoriumStatus == Insolvency.Common.Enums.MoratoriumStatus.Expired;
            ReferenceNumber = debtorDetails.ReferenceNumber;
            DisplayExpiryDate = IsActive && debtorDetails.EndsOn.HasValue;
            CancellationReason = debtorDetails.CancellationReason;
        }

        private string SetMoratoriumStatus(MoratoriumStatus status)
        {
            switch (status)
            {
                case Insolvency.Common.Enums.MoratoriumStatus.Draft:
                    return "Inactive";
                case Insolvency.Common.Enums.MoratoriumStatus.Cancelled:
                case Insolvency.Common.Enums.MoratoriumStatus.Expired:
                    return "Ended";
                default:
                    return status.ToString();
            }
        }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public IEnumerable<ClientName> PreviousNames { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsInMentalHealthMoratorium { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {Surname}";
        public string FormattedDob => DateOfBirth.ToString(Constants.UkDateFullMonthFormat);

        public string MoratoriumType => IsInMentalHealthMoratorium ? "mental health" : "standard";
        public string CapitalizedMoratoriumType => IsInMentalHealthMoratorium ? "Mental Health" : "Standard";

        public string MoratoriumStatus { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnded { get; set; }
        public string ReferenceNumber { get; set; }

        public DateTimeOffset? ActiveMoratoriumStartDate { get; set; }
        public string FormattedActiveMoratoriumStartDate => ActiveMoratoriumStartDate?.ToString(Constants.PrettyDateFormat);

        public DateTimeOffset? ActiveMoratoriumEndDate { get; set; }
        public string FormattedActiveMoratoriumEndDate => ActiveMoratoriumEndDate?.ToString(Constants.PrettyDateFormat);

        public int MoratoriumLength => ActiveMoratoriumEndDate.HasValue
            ? (ActiveMoratoriumEndDate.Value - ActiveMoratoriumStartDate.Value).Days
            : default;
        public string RanFor => $"{MoratoriumLength} day{(MoratoriumLength != 1 ? "s" : "")}";


        public int MoratoriumCurrentDay()
        {
            var now = DateTimeOffset.Now;

            if (!DisplayBreathingSpaceDates)
            {
                return 0;
            }

            if (now < ActiveMoratoriumStartDate.Value.Date)
                return 0;

            return (now - ActiveMoratoriumStartDate.Value).Days + 1;
        }

        public bool DisplayBreathingSpaceDates => ActiveMoratoriumStartDate != null;
        public bool EligibleForReview => MoratoriumCurrentDay() < 21;
        public bool DisplayExpiryDate { get; set; }
        public string CancellationReason { get; set; }
    }
}
