using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Identity.Extensions
{
    public static class AuthenticationExtensions
    {
        private const string AcrMfaChallengeClaimValue = "acr-gg-1";

        public static AuthenticationBuilder AddScpOpenIdConnect(this AuthenticationBuilder builder, Action<OpenIdConnectOptions> configure)
        {
            return builder.AddOpenIdConnect(Constants.ScpAuthenticationSchemeAlias, opt => ConfigureOptions(opt, configure));
        }

        private static void ConfigureOptions(OpenIdConnectOptions options, Action<OpenIdConnectOptions> configure)
        {
            configure(options);
            options.ResponseType = Constants.ResponseType;
            options.ResponseMode = Constants.ResponseMode;
            options.CallbackPath = Constants.CallbackPath;
            options.ConfigureScpOpenIdProfileScopes();

            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            ((JwtSecurityTokenHandler)options.SecurityTokenValidator).MapInboundClaims = false;

            options.Events.OnAuthorizationCodeReceived = OnAuthorizationCodeReceived;
            options.Events.OnTokenResponseReceived = OnTokenResponseReceived;
            options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;
            options.Events.OnTokenValidated = OnTokenValidated;
        }

        private static Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            var passcode = $"{context.Options.ClientId}:{context.Options.ClientSecret}";
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(passcode);
            var authHeaderValue = Convert.ToBase64String(plainTextBytes);

            context.Backchannel.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
            return Task.CompletedTask;
        }

        private static Task OnTokenResponseReceived(TokenResponseReceivedContext context)
        {
            context.Options.Backchannel.DefaultRequestHeaders.Authorization = null;
            return Task.CompletedTask;
        }

        private static Task OnRedirectToIdentityProvider(RedirectContext context)
        {
            if (context.Properties.GetParameter<bool>(Insolvency.Interfaces.Constants.ShouldUseMfaKey))
            {
                context.ProtocolMessage.SetParameter("acr_values", AcrMfaChallengeClaimValue);
            }            
            return Task.CompletedTask;
        }

        private static Task OnTokenValidated(TokenValidatedContext context)
        {
            var acrClaim = context.Principal.FindFirst("acr");
            if (acrClaim == null)
            {
                context.Fail("Unauthorized!");
            }

            if (string.IsNullOrEmpty(acrClaim.Value))
            {
                context.Fail("Unauthorized!");
            }

            if (context.Properties.GetParameter<bool>(Insolvency.Interfaces.Constants.ShouldUseMfaKey) && acrClaim.Value.Trim().ToLower() != AcrMfaChallengeClaimValue)
            {
                context.Fail("Unauthorized!");
            }

            return Task.CompletedTask;
        }




        private static void ConfigureScpOpenIdProfileScopes(this OpenIdConnectOptions options)
        {
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.GetClaimsFromUserInfoEndpoint = true;
        }
    }
}
