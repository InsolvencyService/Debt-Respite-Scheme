using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Identity.Onboarding.Controllers
{    
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            return Problem();
        }
       
    }
}
