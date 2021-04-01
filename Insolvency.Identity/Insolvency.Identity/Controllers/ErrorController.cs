using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Identity.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View("Error");
        }
    }
}
