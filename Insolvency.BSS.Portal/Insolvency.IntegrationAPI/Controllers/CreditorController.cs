using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Insolvency.IntegrationAPI.Controllers
{
    [Route("/OnboardedCreditors")]
    [SwaggerGroup("v1", "raw_v1")]
    [ApiController]
    [Authorize(Policy = Constants.Auth.CommonPolicy)]
    public class CreditorsController : BaseController
    {
        public ICacheClient CacheClient { get; }

        private readonly ICommonDynamicsGateway _dynamicsGateway;
        private readonly ILogger<CreditorsController> _logger;

        public CreditorsController(ICacheClient cacheClient, ICommonDynamicsGateway dynamicsGateway, ILogger<CreditorsController> logger)
        {
            CacheClient = cacheClient;
            _dynamicsGateway = dynamicsGateway;
            _logger = logger;
        }

        [HttpPost("customaddress")]
        public async Task<IStatusCodeActionResult> CreditorAddressAsync(CustomAddress model)
        {
            var id = await _dynamicsGateway.AddAdHocCreditorAddressAsync(model);
            return Ok(id);
        }

        [HttpGet("search")]
        public async Task<CreditorSearchResponse> CreditorSearchAsync([FromQuery] string creditorName)
        {
            var result = await _dynamicsGateway.SearchCmpCreditors(creditorName);
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Creditor>> GetCreditor([FromRoute] Guid id)
        {
            var result = await _dynamicsGateway.GetGenericCreditorById(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("adhoc")]
        public async Task<IActionResult> CreateAdHocCreditor(AdHocCreditorRequst model)
        {
            var result = await _dynamicsGateway.CreateAdHocCreditor(model.Name);
            return Ok(result);
        }

        [HttpGet("")]
        [SwaggerGroup("ma_v1", "raw_ma_v1", "cr_v1", "raw_cr_v1")]
        public async Task<IEnumerable<CreditorSearchDetailedResponse>> GetAllCreditors()
        {
            var result = await CacheClient.GetCachedDataAsync<List<CreditorSearchDetailedResponse>>(Constants.CMPCacheListKey);
            return result;
        }
    }
}
