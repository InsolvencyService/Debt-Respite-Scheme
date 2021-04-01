using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Insolvency.IntegrationAPI.Authorization
{
    public class OrganisationAuthorizationRequirement : IAuthorizationRequirement
    {
        public List<string> OrganisationTypeNames { get; set; }
    }
}
