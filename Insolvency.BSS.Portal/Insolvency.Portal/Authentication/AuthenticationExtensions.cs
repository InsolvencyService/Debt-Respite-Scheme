using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Insolvency.Common;

namespace Insolvency.Portal.Authentication
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder ConfigureAuthentication(this IServiceCollection services, Action<OpenIdConnectOptions> configure, int authenticationLifetimeFromHours)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultScheme = Constants.Auth.CookiesAuthenticationScheme;
                options.DefaultChallengeScheme = Constants.Auth.OpenIdAuthenticationScheme;
            })
            .AddCookie(Constants.Auth.CookiesAuthenticationScheme, opt =>
            {
                opt.AccessDeniedPath = "/Account/SelectOrganisation";
                opt.EventsType = typeof(CookieEvents);
                opt.SlidingExpiration = false;
                opt.ExpireTimeSpan = TimeSpan.FromHours(authenticationLifetimeFromHours);
            })
            .AddOpenIdConnect(Constants.Auth.OpenIdAuthenticationScheme, x =>
            {
                x.Scope.Clear();
                configure(x);
                x.SaveTokens = true;

                x.Events.OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    context.ProtocolMessage.IdTokenHint = null;
                    return Task.CompletedTask;
                };
                x.Events.OnTicketReceived = (context) =>
                {
                    context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(authenticationLifetimeFromHours);
                    context.Properties.AllowRefresh = false;
                    context.Properties.IsPersistent = true;
                    return Task.CompletedTask;
                };
            });
        }
    }
}
