using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using Insolvency.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Authentication
{
    public class ScpAuthenticationProcessor
    {
        private readonly ILogger<ScpAuthenticationProcessor> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScpAuthenticationProcessor(ILogger<ScpAuthenticationProcessor> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public virtual async Task<ScpAuthenticationResponse> ProcessScpAuthenticationAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var result = await httpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            LogExternalAuthentication(result);
            if (result?.Succeeded != true)
            {
                return new ScpAuthenticationResponse { IsSuccessful = false };
            }

            var user = new InsolvencyUser(result.Principal);

            var scpClaimsValidationResult = new Validation.InsolvencyUserValidator().Validate(user);
            if (!scpClaimsValidationResult.IsValid)
            {
                _logger.LogError("Error during processing SCP Principal. Unable to extract all required SCP claims from the SCP Principal.");
                _logger.LogError(String.Join(", ", scpClaimsValidationResult.Errors.Select(p => p.ErrorMessage)));
                return new ScpAuthenticationResponse { IsSuccessful = false };
            }

            var response = new ScpAuthenticationResponse
            {
                InsolvencyUser = user,
                ReturnUrl = result.Properties.Items["returnUrl"] ?? "~/",
                IsSuccessful = true
            };

            var idToken = result.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                response.AuthenticationProperties.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }

            return response;
        }
        private void LogExternalAuthentication(AuthenticateResult result)
        {
            if (result?.Succeeded == true)
            {
                _logger.LogInformation("External authentication successfil.");
                return;
            }

            if (result?.Failure != null)
            {
                _logger.LogError(result.Failure, "External authentication error.");
                return;
            }

            _logger.LogError("External authentication error.");
        }
    }
}
