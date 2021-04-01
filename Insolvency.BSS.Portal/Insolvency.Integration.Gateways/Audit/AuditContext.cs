using System.Collections.Generic;
using Insolvency.Common;
using Insolvency.Common.Identity.Claims.Types;
using Insolvency.Integration.Interfaces.Models;

namespace Insolvency.Integration.Gateways.Audit
{
    public class AuditContext
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public OrganisationClaimType Organisation { get; set; }

        public string CreditorOrganisationId =>
            Organisation?.OrganisationTypeName == Constants.Auth.CreditorOrginisationType
            ? Organisation.Id
            : null;

        public string MoneyAdviserOrganisationId =>
            Organisation?.OrganisationTypeName == Constants.Auth.MoneyAdviserOrginisationType
            ? Organisation.Id
            : null;

        public string SenderName { get; set; }

        public IDictionary<string, object> GenerateActionParameters() =>
            new Dictionary<string, object>().SetDynamicsActionAuditParameters(this);

        public AuditDetail ToAuditDetail(IDictionary<string, object> content)
        {
            return new AuditDetail
            {
                Email = Email,
                Name = Name,
                ActionName = ActionName,
                ControllerName = ControllerName,
                ClientId = ClientId,
                OrganisationId = Organisation?.Id,
                SenderName = SenderName,
                Content = content
            };
        }
    }
}
