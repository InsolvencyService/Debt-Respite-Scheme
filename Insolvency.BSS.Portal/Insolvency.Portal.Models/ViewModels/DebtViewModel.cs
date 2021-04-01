using System;
using Insolvency.Common;
using Insolvency.Common.Enums;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtViewModel
    {
        public virtual Guid Id { get; set; } = default;
        public virtual string DebtAmount { get; set; }

        public virtual string Reference { get; set; }

        public virtual string NINO { get; set; }

        public virtual Guid? SelectedDebtTypeId { get; set; }
        public virtual string SelectedDebtTypeName { get; set; } = "Debt";

        public virtual Guid CreditorId { get; set; }
        public virtual string CreditorName { get; set; }

        public virtual Guid MoratoriumId { get; set; }

        public virtual DateTimeOffset CreatedOn { get; set; }
        public virtual DateTimeOffset? RemovedOn { get; set; }
        public virtual DateTimeOffset? CommencementDate { get; set; }
        public virtual DebtStatus Status { get; set; }
        public virtual Guid? SoldToCreditorId { get; set; }
        public virtual string SoldToCreditorName { get; set; }
        public virtual bool PreviouslySold { get; set; }
        public virtual string FormattedCreatedOn => CreatedOn.ToString(Constants.PrettyDateFormat);
        public virtual string FormattedCommencementDate => CommencementDate?.ToString(Constants.PrettyDateFormat);
        public virtual string FormattedCreatedOnTime => CreatedOn.ToString("t").ToLower();
        public virtual string FormattedCommencementDateTime => CommencementDate?.ToString("t").ToLower();
        public virtual string FormattedCreatedOnOrdinal => CreatedOn.ToOrdinalDateTimeFormat();
        public virtual string FormattedRemovedOnOrdinal => RemovedOn.HasValue ? RemovedOn.Value.ToOrdinalDateTimeFormat() : string.Empty;
        public virtual string FormattedCommencementDateOrdinal => CommencementDate.HasValue ? CommencementDate.Value.ToOrdinalDateTimeFormat() : string.Empty;

        public bool HasDebtTypeName => !string.IsNullOrEmpty(SelectedDebtTypeName);
        public bool HasDebtAmount => !string.IsNullOrEmpty(DebtAmount) && DebtAmount != Constants.TwoDecimalPointFormat;
        public bool HasReference => !string.IsNullOrEmpty(Reference);
        public bool HasNi => !string.IsNullOrEmpty(NINO);
        public bool HasProposedCreditor => !string.IsNullOrEmpty(SoldToCreditorName);

        public string ReturnAction { get; set; }

        public DebtRemovalReason? RemovalReason { get; set; }
        public string AdditionalRemovalInformation { get; set; }
    }
}
