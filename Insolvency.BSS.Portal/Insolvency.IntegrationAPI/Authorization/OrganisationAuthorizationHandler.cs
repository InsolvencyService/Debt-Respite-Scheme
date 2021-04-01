using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Insolvency.IntegrationAPI.Authorization
{
    public class OrganisationAuthorizationHandler : AuthorizationHandler<OrganisationAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganisationAuthorizationHandler(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganisationAuthorizationRequirement requirement)
        {
            var selectedOrganisation = _httpContextAccessor.HttpContext.GetOrganisation();
            if (selectedOrganisation == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (!requirement.OrganisationTypeNames.Any( organisationType => selectedOrganisation.OrganisationTypeName.Equals(organisationType, StringComparison.InvariantCultureIgnoreCase)))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
