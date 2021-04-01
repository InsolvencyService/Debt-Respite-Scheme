using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Identity.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult RedirectToErrorPage(this Controller controller)
        {
            return controller.RedirectToAction("Index", "Error");
        }

        public static IActionResult RedirectToAccessDeniedPage(this Controller controller)
        {
            return controller.RedirectToAction("AccessDenied", "Account");
        }
    }
}
