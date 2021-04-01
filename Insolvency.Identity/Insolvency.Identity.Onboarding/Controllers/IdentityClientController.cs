using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Insolvency.Identity.Onboarding.ViewModels;
using Insolvency.Interfaces.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Onboarding.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityClientController : ControllerBase
    {
        private readonly IIdentityServerRepository _iIdentityServerRepository;
        private readonly ILogger<IdentityClientController> _logger;

        public IdentityClientController(
            IIdentityServerRepository iIdentityServerRepository, 
            ILogger<IdentityClientController> logger)
        {
            _iIdentityServerRepository = iIdentityServerRepository ?? throw new ArgumentNullException(nameof(iIdentityServerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateCodeClient(IdentityClientViewModel clientViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newClientName = $"CodeClient-System-{Guid.NewGuid():N}";
            var newClient = clientViewModel.MapToClientEntity(newClientName);
            var result = await _iIdentityServerRepository.CreateIdentityClientAsync(newClient);

            if (result.IsError)
            {
                return StatusCode(result.OperationError.Code, new { message = result.OperationError.Message, clientId = result.Value.ClientId } );
            }

            return CreatedAtAction(nameof(CreateCodeClient), new { secret = result.Value.Secret, clientId = result.Value.ClientId });
        }
    }
}
