using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Insolvency.Common;
using Insolvency.Common.Identity.Claims.Types;
using Insolvency.Integration.Gateways.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Insolvency.IntegrationAPI
{
    public static class HttpContextExtensions
    {
        private const string SenderAppName = "Bss.Integration.Api";

        public static void SetOrganisation(
            this HttpContext httpContext,
            OrganisationClaimType organisation)
            => httpContext.Items.Add(Constants.OrganisationItemsKey, organisation);

        public static OrganisationClaimType GetOrganisation(this HttpContext httpContext)
            =>
             httpContext.Items.ContainsKey(Constants.OrganisationItemsKey)
                ? (OrganisationClaimType)httpContext.Items[Constants.OrganisationItemsKey]
                : null;

        public static AuditContext GetAuditContext(this HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }
            var result = new AuditContext();
            var routeData = httpContext.GetRouteData();

            if (routeData.Values.ContainsKey("action"))
            {
                result.ActionName = routeData.Values["action"]?.ToString();
                result.ControllerName = routeData.Values["controller"]?.ToString();
            }

            var claims = httpContext.User.Claims;

            result.ClientId = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.ClientId)?.Value;
            result.Name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value;
            result.Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            result.SenderName = SenderAppName;
            result.Organisation = httpContext.GetOrganisation();

            return result;
        }
    }
}
