using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Insolvency.Identity.Onboarding.ViewModels;
using Insolvency.Interfaces.IdentityManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Onboarding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingController : ControllerBase
    {
        private readonly IIdentityManagementRepository _iIdentityManagementRepository;
        private readonly ILogger<OnboardingController> _logger;

        public OnboardingController(IIdentityManagementRepository identityManagementRepository, ILogger<OnboardingController> logger)
        {
            _iIdentityManagementRepository = identityManagementRepository ?? throw new ArgumentNullException(nameof(identityManagementRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(OrganisationPreparationViewModel organisationViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newOrganisation = organisationViewModel.MapToOrganisationEntity();
            var result = await _iIdentityManagementRepository.PrepareOrganisationAsync(newOrganisation);
            if (!result.IsSuccess)
            {
                return StatusCode(result.OperationError.Code, new { message = result.OperationError.Message, externalId = newOrganisation.ExternalId });
            }

            return CreatedAtAction(nameof(Post), new { id = newOrganisation.Id, externalId = newOrganisation.ExternalId });
        }
    }
}
