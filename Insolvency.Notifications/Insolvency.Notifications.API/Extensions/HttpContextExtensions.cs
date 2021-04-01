using System;
using Microsoft.AspNetCore.Http;

namespace Insolvency.Notifications.API.Extensions
{
    public static class HttpContextExtensions
    {
        public static void SetOrganisationId(this HttpContext httpContext, Guid organisationId)
        {
            httpContext.Items.Add(Constants.OrganisationIdItemsKey, organisationId);
        }

        public static Guid? GetOrganisationId(this HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey(Constants.OrganisationIdItemsKey))
            {
                return (Guid)httpContext.Items[Constants.OrganisationIdItemsKey];
            }
            return null;
        }
    }
}
