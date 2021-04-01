using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Insolvency.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Insolvency.Portal.Authentication;
using Insolvency.Portal.Extensions;
using Insolvency.Portal.Models;

namespace Insolvency.Portal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public bool SessionSetEnabled { get; }

        private readonly RefreshTokenOptions _refreshTokenOptions;

        public AccountController(IConfiguration configuration, RefreshTokenOptions refreshTokenOptions)
        {
            this.SessionSetEnabled = configuration.GetValue<bool>("SessionSetEnabled");
            _refreshTokenOptions = refreshTokenOptions ?? throw new ArgumentNullException(nameof(refreshTokenOptions));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return SignOut(
                Constants.Auth.CookiesAuthenticationScheme,
                Constants.Auth.OpenIdAuthenticationScheme);
        }

        [HttpGet]
        public ActionResult<SelectOrganisationViewModel> SelectOrganisation([FromQuery] string returnUrl = null, [FromQuery]bool? unauthorized = null)
        {
            var organisations = User.GetAvailableOrganisations();
           
            if (organisations.Count == 1 && HttpContext.Session.GetOrganisation() == null)
            {
                return SelectOrganisation(organisations.Single().Id, returnUrl);
            }

            var viewModel = new SelectOrganisationViewModel
            {
                Organisations = organisations,
                CurrentSelectedOrganisation = HttpContext.Session.GetOrganisation(),
                ReturnUrl = returnUrl,
                RedirectedFromMiddlewear = unauthorized ?? false
            };          

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectOrganisation(Guid id, [FromQuery] string returnUrl)
        {
            var organisation = User.GetAvailableOrganisations()
                               .Where(o => o.Id == id).First();

            HttpContext.Session.SetOrganisation(organisation);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("/");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> TokenLogin(string refreshToken, Guid? organisationId) 
        {
            if (!this.SessionSetEnabled)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest();
            }

            var httpClient = new HttpClient();           
            var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = $"{_refreshTokenOptions.Authority}connect/token",
                ClientId = _refreshTokenOptions.ClientId,
                ClientSecret = _refreshTokenOptions.ClientSecret,
                RefreshToken = refreshToken
            });

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = (JwtSecurityToken)handler.ReadToken(response.IdentityToken);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(jsonToken.Claims, Constants.Auth.CookiesAuthenticationScheme));

            var accessToken = (JwtSecurityToken)handler.ReadToken(response.AccessToken);
            var expiresAt = System.Text.Json.JsonSerializer.Serialize(new DateTimeOffset(accessToken.ValidTo, TimeSpan.Zero)).Trim('"');

            var authenticationProperties = new AuthenticationProperties();
            var authenticationTokens = new List<AuthenticationToken>
            {
                new AuthenticationToken() { Name = "access_token", Value = response.AccessToken },
                new AuthenticationToken() { Name = "expires_at", Value = expiresAt }
            };
            authenticationProperties.StoreTokens(authenticationTokens);

            if (organisationId.HasValue)
            {
                var organisation = principal.GetAvailableOrganisations()
                                   .Where(o => o.Id == organisationId).First();

                HttpContext.Session.SetOrganisation(organisation);
            }

            await HttpContext.SignInAsync(Constants.Auth.CookiesAuthenticationScheme, principal, authenticationProperties);

            return Redirect("/");         
        }

        [HttpGet]
        public IActionResult SetSession(int id)
        {
            if (!this.SessionSetEnabled)
            {
                return NotFound();
            }

            var model = new SetSessionModel
            {
                Values = Enumerable.Range(0, id).Select(x => new SessionValueModel()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SetSession(SetSessionModel model)
        {
            if (!this.SessionSetEnabled)
            {
                return NotFound();
            }

            foreach (var item in model.Values)
            {
                this.HttpContext.Session.SetString(item.Name, item.Value);
                this.TempData[item.Name] = item.Value;
            }
            return Redirect(model.RedirectUrl);
        }
    }
}
