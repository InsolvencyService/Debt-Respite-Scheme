using System.Collections.Generic;
using Insolvency.Identity.Models;
using Microsoft.AspNetCore.Authorization;

namespace Insolvency.Management.Authorization.Requirements
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public List<RoleType> RoleTypes { get; set; }

        public RoleRequirement(List<RoleType> roleTypes)
        {
            this.RoleTypes = roleTypes;
        }
    }
}
