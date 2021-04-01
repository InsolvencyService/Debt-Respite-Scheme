using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insolvency.Portal.Authorization
{
    public class OrganisationAuthorizationRequirement : IAuthorizationRequirement
    {
        public string OrganisationTypeName { get; set; }
    }
}
