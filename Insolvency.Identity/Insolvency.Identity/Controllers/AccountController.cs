using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Insolvency.Identity.Authentication;
using Insolvency.Identity.Extensions;
using Insolvency.Identity.Models;
using Insolvency.Identity.Security;
using Insolvency.Interfaces.IdentityManagement;
using Insolvency.Models.Audit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly ILogger<AccountController> _logger;
        private readonly IIdentityManagementRepository _iIdentityManagementRepository;
        private readonly ScpAuthenticationProcessor _scpAuthenticationProcessor;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            ILogger<AccountController> logger,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityManagementRepository identityManagementRepository,
            ScpAuthenticationProcessor scpAuthenticationProcessor,
            IEventService events)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _logger = logger;
            _events = events;
            _scpAuthenticationProcessor = scpAuthenticationProcessor ?? throw new ArgumentNullException(nameof(scpAuthenticationProcessor));
            _iIdentityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                _logger.LogError($"User supplied invalid return url: {returnUrl}. Possibly a misconfiguration or malicious attempt for an attack.");
                return this.RedirectToErrorPage();
            }

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var shouldUseMfa = context.Client.Properties.Where(p => p.Key == Insolvency.Interfaces.Constants.ShouldUseMfaKey).FirstOrDefault().Value;

            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", Constants.ScpAuthenticationSchemeAlias }
                }
            };

            if (!string.IsNullOrEmpty(shouldUseMfa))
            {
                props.SetParameter<bool>(Insolvency.Interfaces.Constants.ShouldUseMfaKey, bool.Parse(shouldUseMfa));
            }

            return Challenge(props, Constants.ScpAuthenticationSchemeAlias);
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var scpAuthenticationResponse = await _scpAuthenticationProcessor.ProcessScpAuthenticationAsync();
            if (!scpAuthenticationResponse.IsSuccessful)
            {
                return this.RedirectToErrorPage();
            }
            var user = scpAuthenticationResponse.InsolvencyUser;
            HttpContext.SetAuditDetail(new AuditDetail() 
            { 
                Email = user.Email,
                Name = user.Name,
                ActionName = nameof(Callback),
                ControllerName = nameof(AccountController),
                ClientId = "AuthorizationServer",
                OrganisationId = $"ScpGroupId-{user.ScpGroupId}",
                SenderName = "INSS.Identity"
            });
            await _iIdentityManagementRepository.CompleteOnboardingForPendingOrganisationsAsync(user.Email, user.ScpGroupId);

            var organisationsThatUserIsPartOf = await _iIdentityManagementRepository.GetOrganisationByScpGroupIdAsync(user.ScpGroupId);

            if (!organisationsThatUserIsPartOf.Any())
            {
                await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
                return this.RedirectToAccessDeniedPage();
            }

            await HttpContext.SignInAsync(scpAuthenticationResponse.InsolvencyUser, scpAuthenticationResponse.AuthenticationProperties);
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            var context = await _interaction.GetAuthorizationContextAsync(scpAuthenticationResponse.ReturnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(Constants.ScpAuthenticationSchemeAlias, scpAuthenticationResponse.InsolvencyUser.SubjectId, scpAuthenticationResponse.InsolvencyUser.SubjectId, scpAuthenticationResponse.InsolvencyUser.SubjectId, true, context?.Client.ClientId));

            return Redirect(scpAuthenticationResponse.ReturnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (User?.Identity.IsAuthenticated != true)
            {
                return ScpSignOut(logoutId);               
            }

           
            await HttpContext.SignOutAsync();
            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            var logoutContext = await _interaction.GetLogoutContextAsync(logoutId);

            var loggedOutViewModel = new LoggedOutViewModel();
            loggedOutViewModel.LogoutId = logoutId;
            loggedOutViewModel.SignOutIframeUrl = logoutContext?.SignOutIFrameUrl;

            return View("LoggedOut", loggedOutViewModel);
        }

        [HttpGet]
        public IActionResult CompleteLogout(string logoutId)
        {
            return ScpSignOut(logoutId);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [NonAction]
        private SignOutResult ScpSignOut(string logoutId)
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Logout", new { logoutId = logoutId }),
            };
            return SignOut(authenticationProperties, Constants.ScpAuthenticationSchemeAlias);
        }
    }
}