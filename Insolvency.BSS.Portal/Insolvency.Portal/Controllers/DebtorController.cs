using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Insolvency.Portal.Models;

namespace Insolvency.Portal.Controllers
{
    [Authorize]
    public class DebtorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SignOut()
        {
            var callbackUrl = Url.Action("LogOut", "Account", values: null, protocol: Request.Scheme);
            var foo = SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                AzureADDefaults.OpenIdScheme);
            return foo;
        }
    }
}
