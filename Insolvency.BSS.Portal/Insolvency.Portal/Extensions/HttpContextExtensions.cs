using IdentityModel;
using Microsoft.AspNetCore.Http;
using Insolvency.Portal.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Insolvency.Portal.Extensions
{
    public static class HttpContextExtensions
    {
        public static PortalUser GetPortalUser(this HttpContext context)
        {
            var user = new PortalUser();
            user.IsAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

            if (!user.IsAuthenticated)
            {
                return user;
            }
           
            user.Name = context.User.FindFirstValue(JwtClaimTypes.Name);
            user.Email = context.User.FindFirstValue(JwtClaimTypes.Email);
            user.SelectedOrganisationName = context.Session.GetOrganisation().Name;

            return user;

        }
    }
}
