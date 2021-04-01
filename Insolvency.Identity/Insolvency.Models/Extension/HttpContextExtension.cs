using System.Linq;
using System.Security.Claims;
using IdentityModel;
using Insolvency.Identity.Models.Claims.Types;
using Insolvency.Models.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Insolvency.Models.Extension
{
    public static class HttpContextExtension
    {
        private const string OrganisationItemsKey = "OrganisationItem";

        public static void SetOrganisation(
            this HttpContext httpContext,
            OrganisationClaimType organisation)
            => httpContext.Items.Add(OrganisationItemsKey, organisation);

        public static OrganisationClaimType GetOrganisation(this HttpContext httpContext)
            =>
             httpContext.Items.ContainsKey(OrganisationItemsKey)
                ? (OrganisationClaimType)httpContext.Items[OrganisationItemsKey]
                : null;

        public static AuditDetail GetAuditDetail(this HttpContext httpContext, string senderAppName)
        {
            var audit = new AuditDetail() 
            { 
                SenderName = senderAppName,
                OrganisationId = httpContext.GetOrganisation()?.Id
            };
            var routeData = httpContext.GetRouteData();

            if (routeData.Values.ContainsKey("action"))
            {
                audit.ActionName = routeData.Values["action"]?.ToString();
                audit.ControllerName = routeData.Values["controller"]?.ToString();
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var claims = httpContext.User.Claims;

                audit.ClientId = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.ClientId)?.Value;
                audit.Name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value;
                audit.Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            }

            return audit;
        }
    }
}
