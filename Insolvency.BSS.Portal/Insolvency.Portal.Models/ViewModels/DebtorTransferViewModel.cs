using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorTransferViewModel
    {
        public DebtorTransferViewModel() { }
        public DebtorTransferViewModel(DebtorAccountSummaryViewModel accountSummaryViewModel)
        {
            Reference = accountSummaryViewModel.DebtorDetail.PersonalDetail.ReferenceNumber;
            FirstName = accountSummaryViewModel.DebtorDetail.PersonalDetail.FirstName;
            MiddleName = accountSummaryViewModel.DebtorDetail.PersonalDetail.MiddleName;
            LastName = accountSummaryViewModel.DebtorDetail.PersonalDetail.Surname;
            ActiveMoratoriumStartDate = accountSummaryViewModel.DebtorDetail.PersonalDetail.ActiveMoratoriumStartDate;
            ActiveMoratoriumEndDate = accountSummaryViewModel.DebtorDetail.PersonalDetail.ActiveMoratoriumEndDate;
            MoneyAdviceOrganisation = accountSummaryViewModel.MoneyAdviceOrganisation;
        }

        public DebtorTransferViewModel(DebtorPersonalDetailViewModel personalDetail,
            MoneyAdviceOrganisationViewModel moneyAdviceOrganisation,
            DebtorTransferResponse debtorTransfer)
        {
            if (debtorTransfer is null)
            {
                return;
            }

            Reference = personalDetail.ReferenceNumber;
            FirstName = personalDetail.FirstName;
            MiddleName = personalDetail.MiddleName;
            LastName = personalDetail.Surname;
            ActiveMoratoriumStartDate = personalDetail.ActiveMoratoriumStartDate;
            ActiveMoratoriumEndDate = personalDetail.ActiveMoratoriumEndDate;
            MoneyAdviceOrganisation = moneyAdviceOrganisation;
            TransferReason = debtorTransfer.ReasonForTransfer;
            RequestedOn = debtorTransfer.RequestedOn;
            RequestingMoneyAdviceOrganisation = debtorTransfer.RequestingOrganisation;
            TransferredOn = debtorTransfer.TransferredOn;
            TransferringMoneyAdviserOrganisation = debtorTransfer.TransferringOrganistion;
            Status = debtorTransfer.Status;
        }

        [Required(ErrorMessage = "Please provide details for the transfer")]
        public string TransferReason { get; set; }
        public string Reference { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public DateTimeOffset? ActiveMoratoriumStartDate { get; set; }
        public string FormattedActiveMoratoriumStartDate => ActiveMoratoriumStartDate?.ToString(Constants.AbbreviatedMonthFormat, CultureInfo.InvariantCulture);
        public DateTimeOffset? ActiveMoratoriumEndDate { get; set; }
        public string FormattedActiveMoratoriumEndDate => ActiveMoratoriumEndDate?.ToString(Constants.AbbreviatedMonthFormat, CultureInfo.InvariantCulture);
        public MoneyAdviceOrganisationViewModel MoneyAdviceOrganisation { get; set; }
        public TransferDebtorRequestStatusCodes Status { get; set; }
        public string RequestingMoneyAdviceOrganisation { get; set; }
        public string TransferringMoneyAdviserOrganisation { get; set; }
        public DateTimeOffset? TransferredOn { get; set; }
        public string TransferredBy => $"{TransferringMoneyAdviserOrganisation}, {TransferredOn?.ToString(Constants.UkDateFullMonthFormat)} at {TransferredOn?.ToString("hh:mmtt").ToLower()}";
        public DateTimeOffset? RequestedOn { get; set; }
        public string RequestedBy => $"{RequestingMoneyAdviceOrganisation}, {RequestedOn?.ToString(Constants.UkDateFullMonthFormat)} at {RequestedOn?.ToString("hh:mmtt").ToLower()}";
    }
}
