using System;
using System.Threading.Tasks;
using Insolvency.Notifications.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Insolvency.Notifications.API.Authorization
{
    public class OrganisationHandler : AuthorizationHandler<OrganisationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganisationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganisationRequirement requirement)
        {
            var selectedOrganisationId = _httpContextAccessor.HttpContext.GetOrganisationId();
            if (!selectedOrganisationId.HasValue)
            {
                context.Fail();
                return Task.CompletedTask;
            }           

            context.Succeed(requirement);
            return Task.CompletedTask;
        }        
    }
}
