using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Insolvency.Portal.Authentication
{
    public class CookieEvents : CookieAuthenticationEvents
    {
        private TimeSpan _accessTokenExpirationThreshold;

        public CookieEvents(TimeSpan accessTokenExpirationThreshold)
        {
            _accessTokenExpirationThreshold = accessTokenExpirationThreshold;
        }

        public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var expiresAt = context.Properties.GetTokens().First(token => token.Name.Equals("expires_at")).Value;
            var expiresAtDateTime = DateTimeOffset.Parse(expiresAt);
            var expiresThreshold = TimeSpan.Zero;

            if (context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
            {
                expiresThreshold = _accessTokenExpirationThreshold;
            }

            if (expiresAtDateTime - DateTimeOffset.UtcNow < expiresThreshold)
            {
                context.RejectPrincipal();
                context.ShouldRenew = true;
            }
            return Task.CompletedTask;
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.RedirectUri = context.RedirectUri + "&unauthorized=true";
            return base.RedirectToAccessDenied(context);
        }
    }
}
