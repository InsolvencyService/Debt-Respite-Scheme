using System;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models.CreditorService.Requests;
using Insolvency.Integration.Models.CreditorService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Insolvency.IntegrationAPI.Controllers
{
    [SwaggerGroup("v1", "raw_v1")]
    [Route("/Creditor")]
    [ApiController]
    [Authorize(Policy = Constants.Auth.CreditorPolicy)]
    public class CreditorServiceController : BaseController
    {
        public ICacheClient CacheClient { get; }

        private readonly ICreditorServiceDynamicsGateway _creditorGateway;
        private readonly ICommonDynamicsGateway _dynamicsGateway;
        private readonly ILogger<CreditorServiceController> _logger;

        public CreditorServiceController(ICacheClient cacheClient,
                                        ICreditorServiceDynamicsGateway creditorGateway,
                                         ICommonDynamicsGateway dynamicsGateway,
                                         ILogger<CreditorServiceController> logger)
        {
            this.CacheClient = cacheClient;
            _dynamicsGateway = dynamicsGateway;
            _creditorGateway = creditorGateway;
            _logger = logger;
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpGet("BreathingSpaces/{id}")]
        public async Task<BreathingSpaceResponse> GetBreathingSpace([FromRoute] Guid id)
        {
            var creditorId = GetOrganisationId();
            return await _dynamicsGateway.GetCreditorBreathingSpaceAsync(id, creditorId);
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpPost("BreathingSpaces/{id}/EligibilityReviews")]
        public async Task<IStatusCodeActionResult> ClientEligibilityReviews([FromRoute] Guid id, [FromBody] CrDebtorEligibilityReviewRequest model)
        {
            model.MoratoriumId = id;
            model.CreditorId = GetOrganisationId();           

            await _dynamicsGateway.CreateDebtorEligibilityReviewRequest(model);
            return Ok();
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpPost("Debts/{id}/EligibilityReviews")]
        public async Task<IStatusCodeActionResult> DebtEligibilityReviews([FromRoute] Guid id, [FromBody] DebtEligibilityReviewRequest model)
        {
            model.DebtId = id;
            model.CreditorId = GetOrganisationId();

            await _dynamicsGateway.CreateDebtEligibilityReviewRequest(model);
            return Ok();
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpPost("Debts/{id}/ProtectionsApplied")]
        public async Task<IStatusCodeActionResult> ProtectionsApplied([FromRoute] Guid id)
        {
            await _creditorGateway.DebtStopAllAction(id, GetOrganisationId());
            return Ok();
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpPost("Debts/{id}/DebtHasBeenSold")]
        public async Task<IStatusCodeActionResult> DebtSoldOn([FromRoute] Guid id, [FromBody] DebtSoldOnRequest model)
        {
            model.DebtId = id;
            model.CreditorId = GetOrganisationId();

            await _dynamicsGateway.DebtSoldOn(model);
            return Ok();
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpPost("BreathingSpaces/{id}/Debts")]
        public async Task<ActionResult<ProposeDebtResponse>> ProposeADebt([FromRoute] Guid id, [FromBody] ProposeDebtRequest model)
        {
            model.MoratoriumId = id;
            model.CreditorId = GetOrganisationId();

            var result = await _creditorGateway.ProposeADebt(model);
            return Ok(result);
        }

        [SwaggerGroup("raw_cr_v1", "cr_v1")]
        [HttpPost("Debts/{id}/DebtHasBeenSoldToAdHocCreditor")]
        public async Task<IStatusCodeActionResult> DebtSoldOnToAdHocCreditor([FromRoute] Guid id, [FromBody] DebtSoldOnToAdHocCreditorRequest model)
        {
            model.DebtId = id;
            model.CreditorId = GetOrganisationId();
            await _dynamicsGateway.TransferDebtToNewAdhocCreditor(model);
            return Ok();
        }
    }
}
