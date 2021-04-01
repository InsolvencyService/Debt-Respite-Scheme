using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Management.Controllers
{
    public class LogoutController : Controller
    {
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return SignOut(
                Constants.AuthSchemes.CookieAuthSckeme,
                Constants.AuthSchemes.OidcAuthScheme);
        }
    }
}
