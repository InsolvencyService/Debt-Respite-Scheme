using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insolvency.Example.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        [Route("World")]
        public IActionResult World()
        {
            return Ok("Hello World!");
        }

        [HttpGet]
        [Authorize]
        [Route("Application")]
        public IActionResult Application()
        {
            return Ok("Hello Application!");
        }
    }
}
