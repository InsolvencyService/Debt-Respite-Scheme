using System;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Models.MoneyAdviserService.Responses
{
    public class DebtorNominatedContactResponse
    {
        public Guid ContactId { get; set; }
        public Guid RoleId { get; set; }
        public string FullName { get; set; }
        public PointContactRoleType PointContactRole { get; set; }
        public string TelephoneNumber { get; set; }
        public PointContactConfirmationMethod NotificationMethod { get; set; }
        public string EmailAddress { get; set; }
        public AddressResponse CommunicationAddress { get; set; }
    }
}