using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Common.Identity;
using Insolvency.Common.Identity.Claims;
using Insolvency.Common.Identity.Claims.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Insolvency.IntegrationAPI.Middlewares
{
    public class OrganisationSelectorMiddleware
    {
        private readonly RequestDelegate _next;

        public OrganisationSelectorMiddleware(RequestDelegate next, ILogger<OrganisationSelectorMiddleware> logger) => _next = next;

        public Task InvokeAsync(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return _next(httpContext);
            }

            var organisations = httpContext.User.Claims
                               .Where(t => t.Type == InssClaimTypes.Organisation)
                               .Select(c => OrganisationClaimType.FromClaim(c))
                               .ToList();

            if (organisations.Count == 0)
            {
                return _next(httpContext);
            }

            if (organisations.Count == 1)
            {
                httpContext.SetOrganisation(organisations.First());
                return _next(httpContext);
            }

            SetCurrentOrganisationFromHeader(httpContext, organisations);
            return _next(httpContext);
        }

        public void SetCurrentOrganisationFromHeader(HttpContext httpContext, List<OrganisationClaimType> organisations)
        {
            var organisationId = httpContext.Request.Headers[InssHttpHeaderNames.CurrentOrganisationExternalIdHeaderName].FirstOrDefault();
            if (string.IsNullOrEmpty(organisationId))
            {
                return;
            }

            var organisation = organisations
                                   .Where(o => o.Id.Equals(organisationId, StringComparison.InvariantCultureIgnoreCase))
                                   .FirstOrDefault();
            if (organisation == null)
            {
                return;
            }
            httpContext.SetOrganisation(organisation);
        }
    }
}
