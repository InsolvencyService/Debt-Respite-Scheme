using System;
using Microsoft.AspNetCore.Mvc;

namespace Insolvency.IntegrationAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        protected virtual Guid GetOrganisationId() => Guid.Parse(HttpContext.GetOrganisation().Id);

        [NonAction]
        protected virtual string GetOrganisationName() => HttpContext.GetOrganisation().Name;
    }
}
