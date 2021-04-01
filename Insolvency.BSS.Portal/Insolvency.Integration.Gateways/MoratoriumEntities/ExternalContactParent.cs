using System;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class ExternalContactParent
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string Role { get; set; }
        public int SubRoleId { get; set; }
        public ExternalContact ExternalContact { get; set; }

        public DebtorNominatedContactResponse ToDebtorNominatedContactResponse(DynamicsGatewayOptions options)
        {
            return new DebtorNominatedContactResponse
            {
                ContactId = ExternalContact.Id,
                RoleId = Id,
                FullName = $"{ExternalContact.FirstName} {ExternalContact.LastName}",
                TelephoneNumber = ExternalContact.HomeTelephone,
                EmailAddress = ExternalContact.EmailAddress,
                PointContactRole = (PointContactRoleType)SubRoleId,
                NotificationMethod = (PointContactConfirmationMethod)ExternalContact.PreferredChannel,
                CommunicationAddress = ExternalContact.CurrentAddress?.ToAddressResponse()
            };
        }
    }
}