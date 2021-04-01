using System;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels.Creditor
{
    public class CreditorDebtPartialViewModel
    {
        public CreditorDebtPartialViewModel() { }
        public CreditorDebtPartialViewModel(
            DebtDetailResponse debt,
            DateTimeOffset? breathingSpaceEndDate = null,
            bool isInMentalHealthBreathingSpace = false)
        {
            Id = debt.Id;
            Status = debt.Status;
            Amount = debt.Amount?.ToString("0.##");
            Reference = debt.Reference;
            NINO = debt.NINO;
            Type = debt.DebtTypeName;
            StartsOn = debt.CreatedOn;
            EndsOn = debt.EndsOn;
            BreathingSpaceEndDate = breathingSpaceEndDate;
            IsInMentalHealthBreathingSpace = isInMentalHealthBreathingSpace;
            CreditorId = debt.CreditorId;
            SoldToCreditorId = debt.SoldToCreditorId;
            PreviouslySold = PreviouslySold;
        }

        public Guid Id { get; set; }
        public DebtStatus Status { get; set; }
        public string Amount { get; set; }
        public string Reference { get; set; }
        public string NINO { get; set; }
        public string Type { get; set; }

        public DateTimeOffset? StartsOn { get; set; }
        public string FormattedStartsOn => StartsOn?.ToString(Constants.PrettyDateFormat);
        public DateTimeOffset? EndsOn { get; set; }
        public string FormattedEndsOn => EndsOn?.ToString(Constants.PrettyDateFormat);
        public DateTimeOffset? BreathingSpaceEndDate { get; set; }
        public string FormattedBreathingSpaceEndDate => BreathingSpaceEndDate?.ToString(Constants.PrettyDateFormat);

        public bool IsInMentalHealthBreathingSpace { get; set; }

        public bool HasAmount => !string.IsNullOrWhiteSpace(Amount) && Amount != "0";
        public bool HasType => !string.IsNullOrWhiteSpace(Type);
        public bool HasReference => !string.IsNullOrWhiteSpace(Reference);
        public bool HasNINO => !string.IsNullOrWhiteSpace(NINO);

        public Guid? CreditorId { get; set; }
        public Guid? SoldToCreditorId { get; set; }
        public bool PreviouslySold { get; set; }
    }
}
