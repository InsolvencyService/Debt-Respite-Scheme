using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Insolvency.Portal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insolvency.Portal.Authorization
{
    public class OrganisationAuthorizationHandler : AuthorizationHandler<OrganisationAuthorizationRequirement>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public OrganisationAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganisationAuthorizationRequirement requirement)
        {
            var selectedOrganisation = _httpContextAccessor.HttpContext.Session.GetOrganisation();
            if (selectedOrganisation == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (!selectedOrganisation.OrganisationTypeName.Equals(requirement.OrganisationTypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
