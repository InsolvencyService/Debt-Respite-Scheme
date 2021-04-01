using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Common.Exceptions;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.BreathingSpaceCreation;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Insolvency.IntegrationAPI.Controllers
{
    [Route("/MoneyAdviser")]
    [SwaggerGroup("v1", "raw_v1")]
    [ApiController]
    [Authorize(Policy = Constants.Auth.MoneyAdviserPolicy)]
    public class MoneyAdviserServiceController : BaseController
    {
        private readonly IMoneyAdviserServiceDynamicsGateway _moneyAdviserGateway;
        public IDebtorSearchGateway _searchGateway { get; }
        private readonly ILogger<MoneyAdviserServiceController> _logger;
        private readonly ICommonDynamicsGateway _dynamicsGateway;
        private readonly IBreathingSpaceBrowseGateway _breathingSpaceBrowseGateway;

        public MoneyAdviserServiceController(
            IMoneyAdviserServiceDynamicsGateway moneyAdviserGateway,
            IDebtorSearchGateway searchGateway,
            ILogger<MoneyAdviserServiceController> logger,
            ICommonDynamicsGateway dynamicsGateway,
            IBreathingSpaceBrowseGateway breathingSpaceBrowseGateway)
        {
            _moneyAdviserGateway = moneyAdviserGateway;
            _dynamicsGateway = dynamicsGateway;
            _searchGateway = searchGateway;
            _logger = logger;
            _breathingSpaceBrowseGateway = breathingSpaceBrowseGateway;
        }


        [HttpGet("LandingPageStats")]
        public async Task<ActionResult<MoneyAdviserLandingPageStatsResponse>> GetMoneyAdviserLandingPageStats()
        {
            var organisationId = GetOrganisationId();
            var landingPageStats = await _moneyAdviserGateway.GetMoneyAdviserLandingPageStats(organisationId);
            return Ok(landingPageStats);
        }

        [HttpPost("BreathingSpaces/Browse")]
        public async Task<ActionResult<BreathingSpaceBrowseResponse>> BrowseBreathingSpaces([FromBody] BreathingSpaceBrowseRequest request)
        {
            HttpContext.Items.Add(Constants.MaxPageSizeODataKey, true);
            if (request.Page.HasValue && request.Page > 1)
            {
                HttpContext.Items.Add(Constants.RequestedPageKey, request.Page);
            }
            var organisationId = GetOrganisationId();
            var breathingSpaces = await _breathingSpaceBrowseGateway.BrowserBreathingSpaceByAsync(request.Category, request.Task, organisationId, request.ShowNewestFirst);
            return Ok(breathingSpaces);
        }

        [HttpPost("/api/debtor")]
        public async Task<IStatusCodeActionResult> CreateAsync(ClientDetailsCreateRequest model)
        {
            var organisationId = GetOrganisationId();
            var id = await _moneyAdviserGateway.CreateClientWithDetailsAsync(model, organisationId);
            return Ok(id);
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPut("BreathingSpaces/{id}/Debtor")]
        public async Task<IStatusCodeActionResult> UpdateAsync([FromRoute] Guid id, ClientDetailsUpdateRequest model)
        {
            if (id == Guid.Empty)
            {
                throw new NotFoundHttpResponseException();
            }

            model.MoratoriumId = id;

            await _moneyAdviserGateway.UpdateClientWithDetailsAsync(model);
            return Ok();
        }

        [HttpPost("/api/debtor/previousname/add")]
        public async Task<IStatusCodeActionResult> ClientPreviousName(ClientPreviousNameCreateRequest model)
        {
            var id = await _moneyAdviserGateway.AddClientPreviousNameAsync(model);
            return Ok(id);
        }

        [HttpPost("/api/debtor/previousname/update")]
        public async Task<IStatusCodeActionResult> ClientPreviousName(ClientPreviousNameUpdateRequest model)
        {
            var id = await _moneyAdviserGateway.UpdateClientPreviousNameAsync(model);
            return Ok(id);
        }

        [HttpGet("/api/debtor/names/all")]
        public async Task<DebtorDetailsResponse> GetClientNamesSummaryAsync(Guid moratoriumId)
        {
            var result = await _dynamicsGateway.GetClientNamesAsync(moratoriumId);
            return result;
        }

        [HttpPost("/api/debtor/customaddress")]
        public async Task<IStatusCodeActionResult> DebtorAddressAsync(CustomAddress model)
        {
            var id = await _moneyAdviserGateway.CreateDebtorAddressAsync(model);
            return Ok(id);
        }

        [HttpPut("/api/debtor/previous-address")]
        public async Task<IStatusCodeActionResult> UpdatePreviousAddressAsync(UpdateCustomAddress model)
        {
            await _moneyAdviserGateway.UpdateAddressAsync(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPut("BreathingSpaces/{bsid}/Addresses/{aid}")]
        public async Task<IStatusCodeActionResult> UpdateAddressAsync([FromRoute] Guid bsid, [FromRoute] Guid aid, UpdateCustomAddressCurrent model)
        {
            if (bsid == Guid.Empty || aid == Guid.Empty)
            {
                throw new NotFoundHttpResponseException();
            }

            model.AddressId = aid;
            model.MoratoriumId = bsid;
            await _moneyAdviserGateway.UpdateAddressAsync(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("BreathingSpaces/{id}/Debt")]
        public async Task<CreateDebtResponse> DebtorDebtAsync([FromRoute] Guid id, CreateDebtRequest model)
        {
            model.MoratoriumId = id;
            var debtId = await _moneyAdviserGateway.CreateDebtAsync(model);
            return new CreateDebtResponse { DebtId = debtId };
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("BreathingSpaces/Debt/Update")]
        public async Task<CreateDebtResponse> DebtorUpdateDebtAsync(UpdateDebtRequest model)
        {
            var debtId = await _moneyAdviserGateway.UpdateDebtAsync(model);
            return new CreateDebtResponse { DebtId = debtId };
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpGet("BreathingSpaces/Search")]
        public async Task<IEnumerable<AccountSearchResponse>> Search([FromQuery] AccountSearchRequest searchParam)
        {
            var organisationId = GetOrganisationId();
            var result = await _searchGateway.SearchAccountsAsync(searchParam, organisationId);
            return result;
        }

        [HttpPost("/api/debtor/save")]
        public async Task<DebtorAccountSaveResponse> SaveDebtorAsync(DebtorAccountSaveRequest model)
        {
            var result = await _moneyAdviserGateway.SaveDebtorAsync(model);
            return result;
        }

        [HttpGet("/api/debtor/details")]
        public async Task<ActionResult<BreathingSpaceResponse>> GetDebtorDetailsAsync(Guid moratoriumId)
        {
            var organisationId = GetOrganisationId();
            var result = await _dynamicsGateway.GetMaBreathingSpaceAsync(moratoriumId, organisationId);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("/api/debtor/{moratoriumId}/set-as-mental-health")]
        public async Task<IActionResult> SetBreathingSpaceAsMentalHealth([FromRoute] Guid moratoriumId)
        {
            var result = await _moneyAdviserGateway.SetBreathingSpaceAsMentalHealth(moratoriumId);
            return Ok(result);
        }

        [HttpPost("/api/debtor/{moratoriumId}/set-as-standard")]
        public async Task<IActionResult> SetBreathingSpaceAsStandard([FromRoute] Guid moratoriumId)
        {
            var result = await _moneyAdviserGateway.SetBreathingSpaceAsStandard(moratoriumId);
            return Ok(result);
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpGet("BreathingSpaces/{id}")]
        public async Task<ActionResult<BreathingSpaceResponse>> GetDebtorAccountSummaryAsync([FromRoute] Guid id)
        {
            var organisationId = GetOrganisationId();
            var result = await _dynamicsGateway.GetMaBreathingSpaceAsync(id, organisationId);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPut("BreathingSpaces/{id}/Addresses/Hidden")]
        public async Task<IStatusCodeActionResult> DebtorHideAddressAsync([FromRoute] Guid id, DebtorHideAddressRequest model)
        {
            if (id == Guid.Empty)
            {
                throw new NotFoundHttpResponseException();
            }

            model.MoratoriumId = id;

            var updatedAddressId = await _moneyAdviserGateway.DebtorHideAddressAsync(model);
            return Ok(updatedAddressId);
        }

        [HttpPost("/api/debtor/add-business-address")]
        public async Task<ActionResult<BusinessAdressResponse>> DebtorAddBusinessAddressAsync(BusinessAddressRequest model)
        {
            var response = await _moneyAdviserGateway.AddDebtorBusinessAddressAsync(model);
            return Ok(response);
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPut("BreathingSpaces/{bsid}/Businesses/{bid}")]
        public async Task<IStatusCodeActionResult> UpdateBusinessNameAsync([FromRoute] Guid bsid, [FromRoute] Guid bid, BusinessNameUpdateRequest model)
        {
            model.MoratoriumId = bsid;
            model.BusinessId = bid;

            if (bsid == Guid.Empty)
            {
                throw new NotFoundHttpResponseException();
            }

            if (bid == Guid.Empty)
            {
                throw new NotFoundHttpResponseException();
            }

            await _moneyAdviserGateway.UpdateBusinessNameAsync(model);
            return Ok();
        }

        [HttpGet("/api/debtor/nominated-contact-summary")]
        public async Task<DebtorNominatedContactResponse> GetNominatedContactAsync(Guid moratoriumId)
        {
            var result = await _moneyAdviserGateway.GetNominatedContactAsync(moratoriumId);
            return result;
        }

        [HttpPost("/api/debtor/add-nominated-contact")]
        public async Task<ActionResult<NominatedContactCreateResponse>> CreateNominatedContactAsync(NominatedContactCreateRequest model)
        {
            var response = await _moneyAdviserGateway.CreateNominatedContactAsync(model);
            return Ok(response);
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPut("BreathingSpaces/{bsid}/NominatedContacts/{cid}")]
        public async Task<IStatusCodeActionResult> UpdateNominatedContactAsync([FromRoute] Guid bsid, [FromRoute] Guid cid, NominatedContactUpdateRequest model)
        {
            if (bsid == default || cid == default)
            {
                throw new NotFoundHttpResponseException();
            }

            model.ContactId = cid;
            model.MoratoriumId = bsid;

            await _moneyAdviserGateway.UpdateNominatedContactAsync(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("BreathingSpaces/{id}/End")]
        public async Task<IStatusCodeActionResult> DebtorEndAccount([FromRoute] Guid id, DebtorAccountEndRequest model)
        {
            model.MoratoriumId = id;
            await _moneyAdviserGateway.DebtorEndAccount(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPut("BreathingSpaces/{id}/Debtor/ContactPreferences")]
        public async Task<IStatusCodeActionResult> DebtorSetContactPreference([FromRoute] Guid id, DebtorContactPreferenceRequest model)
        {
            if (id == Guid.Empty)
            {
                throw new NotFoundHttpResponseException();
            }

            model.MoratoriumId = id;
            await _moneyAdviserGateway.DebtorSetContactPreference(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("BreathingSpaces/Create")]
        public async Task<BSCreationResponse> CreateBreathingSpace(CreateBreathingSpace moratorium)
        {
            var debtsCount = moratorium.Debts?.Count ?? 0;
            var adHocDebtsCount = moratorium.AdHocDebts?.Count ?? 0;

            if (debtsCount + adHocDebtsCount == 0)
            {
                throw new UnprocessableEntityHttpResponseException("There must be one (1) eligible debt per breathing space");
            }

            var organisationId = GetOrganisationId();
            var result = await _moneyAdviserGateway.CreateBreathingSpace(moratorium, organisationId);
            return result;
        }

        [HttpGet("/api/debtor/debt/{id}")]
        public async Task<DebtDetailResponse> GetDebt([FromRoute] Guid id)
        {
            var result = await _moneyAdviserGateway.GetDebtDetail(id);
            return result;
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("Debts/{id}/EligibilityReviews/MakeDetermination")]
        public async Task<IStatusCodeActionResult> MakeDebtEligibilityDetermination([FromRoute] Guid id, DebtEligibilityReviewResponseRequest model)
        {
            model.DebtId = id;
            var result = await _moneyAdviserGateway.SubmitDebtEligibilityReview(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("BreathingSpaces/{id}/EligibilityReviews/MakeDetermination")]
        public async Task<IStatusCodeActionResult> MakeClientEligibilityDetermination([FromRoute] Guid id, DebtorEligibilityReviewResponseRequest model)
        {
            model.MoratoriumId = id;

            var result = await _moneyAdviserGateway.DebtorReviewClientEligibility(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("BreathingSpaces/{id}/EligibilityReviews")]
        public async Task<IStatusCodeActionResult> DebtorEligibilityReviewTask([FromRoute] Guid id, [FromBody] MaDebtorEligibilityReviewRequest model)
        {
            model.MoratoriumId = id;
            await _dynamicsGateway.CreateDebtorEligibilityReviewRequest(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("Debts/{id}/EligibilityReviews")]
        public async Task<IStatusCodeActionResult> DebtEligibilityReviewTask([FromRoute] Guid id, [FromBody] DebtEligibilityReviewRequest model)
        {
            model.DebtId = id;
            model.MoneyAdviserId = GetOrganisationId();
            await _dynamicsGateway.CreateDebtEligibilityReviewRequest(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("Debts/{id}/DebtHasBeenSold")]
        public async Task<IStatusCodeActionResult> DebtSoldOn([FromRoute] Guid id, [FromBody] DebtSoldOnRequest model)
        {
            model.DebtId = id;
            model.MoneyAdviserId = GetOrganisationId();
            await _dynamicsGateway.DebtSoldOn(model);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("Debts/{id}/AcceptSoldNotification")]
        public async Task<IStatusCodeActionResult> ConfirmDebtSold([FromRoute] Guid id)
        {
            await _moneyAdviserGateway.ConfirmDebtSold(id);
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("Debts/{id}/End")]
        public async Task<IStatusCodeActionResult> RemoveDebt([FromRoute] Guid id, [FromBody] RemoveDebtRequest removeDebtRequest)
        {
            removeDebtRequest.DebtId = id;
            await _moneyAdviserGateway.RemoveDebt(removeDebtRequest);
            return Ok();
        }

        [HttpDelete("/api/debtor/debt/{id}")]
        public async Task<IStatusCodeActionResult> RemoveDebtPresubmission([FromRoute] Guid id)
        {
            if (id == default)
            {
                throw new NotFoundHttpResponseException();
            }

            await _moneyAdviserGateway.RemoveDebtPresubmission(id, GetOrganisationId());
            return Ok();
        }

        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        [HttpPost("Debts/{id}/MakeProposedDebtDetermination")]
        public async Task<IStatusCodeActionResult> MakeProposedDebtDetermination([FromRoute] Guid id, [FromBody] ReviewProposedDebtRequest reviewProposedDebtRequest)
        {
            reviewProposedDebtRequest.DebtId = id;
            await _moneyAdviserGateway.MakeProposedDebtDetermination(reviewProposedDebtRequest);
            return Ok();
        }

        [HttpPost("BreathingSpaces/{id}/AdhocDebts")]
        [SwaggerGroup("ma_v1", "raw_ma_v1")]
        public async Task<IStatusCodeActionResult> CreateAdhocDebt([FromRoute] Guid id, [FromBody] CreateAdHocDebtRequest createAdHocDebtRequest)
        {
            createAdHocDebtRequest.MoratoriumId = id;
            await _moneyAdviserGateway.CreateDebtAndAdHocCreditor(createAdHocDebtRequest);
            return Ok();
        }

        [HttpPost("BreathingSpaces/{id}/Transfer")]
        public async Task<IStatusCodeActionResult> TransferDebtor([FromRoute] Guid id, [FromBody] TransferDebtorRequest transferDebtorRequest)
        {
            transferDebtorRequest.MoratoriumId = id;
            transferDebtorRequest.MoneyAdviceOrganisationId = GetOrganisationId();
            transferDebtorRequest.MoneyAdviceOrganisationName = GetOrganisationName();

            await _moneyAdviserGateway.TransferDebtor(transferDebtorRequest);
            return Ok();
        }

        [HttpPost("BreathingSpaces/{id}/CompleteTransfer")]
        public async Task<IStatusCodeActionResult> CompleteTransferDebtor([FromRoute] Guid id)
        {
            await _moneyAdviserGateway.CompleteTransferDebtor(new TransferDebtorCompleteRequest
            {
                MoratoriumId = id,
                MoneyAdviceOrganisationId = GetOrganisationId(),
                MoneyAdviceOrganisationName = GetOrganisationName(),
            });

            return Ok();
        }

        [HttpPost("/api/debtor/{id}/transfer/acknowledge")]
        public async Task<IStatusCodeActionResult> AcknowledgeDebtorTransfer([FromRoute] Guid id, [FromBody] DebtorTransferRequest debtorTransferRequest)
        {
            debtorTransferRequest.MoratoriumId = id;
            await _moneyAdviserGateway.AcknowledgeDebtorTransfer(debtorTransferRequest);
            return Ok();
        }
    }
}
