using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Insolvency.Portal.Extensions;
using Insolvency.Portal.Models;
using Insolvency.Portal.Services.Banner;

namespace Insolvency.Portal.Controllers
{
    [Authorize(Policy = Constants.Auth.MoneyAdviserPolicy)]
    public class BreathingSpaceController : BaseController
    {
        private const string _bsSummaryViewModel = "bsSummaryViewModel";
        private const string _confirmProposedDebtReview = "ConfirmProposedDebtReview";
        private const string _creditorIdKey = "creditorIdKey";
        private const string _creditorKey = "creditorKey";
        private const string _creditorNameKey = "creditorNameKey";
        private const string _dateValidationMessage = "DateValidationMessage";
        private const string _debtDetailViewModel = "DebtDetailViewModel";
        private const string _debtKey = "debtKey";
        private const string _debtorAddDebtFromCreateBreathingSpace = "DebtorAddDebtFromCreateBreathingSpace";
        private const string _debtorAddNonCmpDebt = "DebtorAddNonCmpDebt";
        private const string _debtorBusinessName = "DebtorBusinessName";
        private const string _debtorDetailViewModel = "DebtorDetailViewModel";
        private const string _debtorInactiveAccountDetails = "DebtorInactiveAccountDetails";
        private const string _debtorEndedAccountDetails = "DebtorEndedAccountDetails";
        private const string _debtorNotOwningAccountDetails = "DebtorNotOwningAccountDetails";
        private const string _debtorNotOwningEndedAccountDetails = "DebtorNotOwningEndedAccountDetails";
        private const string _debtorTransferViewModel = "DebtorTransferViewModel";
        private const string _isDebtTransferred = "IsDebtTransferred";
        private const string _nominatedContactViewModel = "DebtorNominatedContactViewModel";
        private const string _addressHiddenKey = "AddressHiddenForBreathingSpace";
        private readonly ILogger<BreathingSpaceController> _logger;
        private readonly IIntegrationGateway _integrationGateway;
        private readonly IAddressLookupGateway _addressLookupService;
        private readonly CountryList _countryList;
        private readonly BannerService _bannerService;

        public BreathingSpaceController(
            IIntegrationGateway integrationGateway,
            IAddressLookupGateway addressLookupService,
            CountryList countryList,
            ILogger<BreathingSpaceController> logger,
            BannerService bannerService
            ) : base()
        {
            _integrationGateway = integrationGateway;
            _addressLookupService = addressLookupService;
            _countryList = countryList;
            _logger = logger;
            _bannerService = bannerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClearSession();

            var model = await _integrationGateway.GetMoneyAdviserLandingPageStats();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Browse(
            [FromQuery] BreathingSpaceBrowseViewModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
            }

            GenerateNewJourneyId();

            if (model.BrowseByCategory != BreathingSpaceBrowseCategory.TasksToDo)
            {
                model.BrowseByTask = null;
            }
            if (model.BrowseByCategory == BreathingSpaceBrowseCategory.TasksToDo && !model.BrowseByTask.HasValue)
            {
                model.BrowseByTask = BreathingSpaceBrowseTask.DebtsToBeReviewed;
            }

            model.LandingPageModel = await _integrationGateway.GetMoneyAdviserLandingPageStats();
            await _integrationGateway.BrowseBreathingSpacesAsync(model);

            return View(model);
        }

        #region Debtor Account Submit Wizard

        [HttpGet]
        public async Task<IActionResult> ClientDetails(
            [FromQuery] bool edit = false,
            [FromQuery] string autoFocus = null,
            [FromQuery] string returnAction = null)
        {
            var model = new ClientDetailsCreateViewModel { IsValidDateOfBirth = true };

            //check if this is a an intentional edit from a 'Change' button
            if (edit)
            {
                var debtorDetail = TempData[_debtDetailViewModel] != null
                    ? JsonSerializer.Deserialize<DebtorDetailViewModel>(TempData.Peek(_debtorDetailViewModel).ToString())
                    : await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

                var debtorPersonalDetail = debtorDetail.PersonalDetail;
                model.FirstName = debtorPersonalDetail?.FirstName;
                model.MiddleName = debtorPersonalDetail?.MiddleName;
                model.LastName = debtorPersonalDetail?.Surname;
                model.BirthDay = debtorPersonalDetail?.DateOfBirth.Day;
                model.BirthMonth = debtorPersonalDetail?.DateOfBirth.Month;
                model.BirthYear = debtorPersonalDetail?.DateOfBirth.Year;
                model.ReturnAction = returnAction;
                model.AutoFocus = autoFocus;
                model.MoratoriumId = GetMoratoriumId();
            }
            // check if it's journey edit from back button/link navigation
            else
            {
                var storedModel = GetJourneyObject<ClientDetailsCreateViewModel>();
                if (storedModel != null)
                {
                    model = storedModel;
                    TryValidateModel(model);
                }
            }

            model.ReturnAction = nameof(ClientNamesSummary);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientDetails([FromForm] ClientDetailsCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(
                    nameof(ClientDetails),
                    new
                    {
                        edit = false,
                        returnAction = model.ReturnAction
                    });
            }

            if (model.MoratoriumId == default)
            {
                var moratoriumId = await _integrationGateway.CreateClientAsync(model);
                model.MoratoriumId = moratoriumId;
                SetMoratoriumId(moratoriumId);
            }
            else
            {
                await _integrationGateway.UpdateClientAsync(model);
            }

            SetJourneyObject(model);

            return CompleteSubJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public async Task<IActionResult> ClientNamesSummary()
        {
            var model = await _integrationGateway.GetClientNamesAsync(GetMoratoriumId());

            if (string.IsNullOrWhiteSpace(model.CurrentNameDisplay))
            {
                return RedirectToHome();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ClientAddPreviousName(
            [FromQuery] Guid nameId = default,
            [FromQuery] string returnAction = null)
        {
            var model = new ClientAddPreviousNameViewModel();

            if (nameId != default)
            {
                var result = await _integrationGateway.GetClientNamesAsync(GetMoratoriumId());

                var prevName = result.PreviousNames.FirstOrDefault(n => n.NameId == nameId);
                model = new ClientAddPreviousNameViewModel
                {
                    FirstName = prevName?.FirstName,
                    MiddleName = prevName?.MiddleName,
                    LastName = prevName?.LastName,
                    PreviousNameId = nameId
                };
            }
            else
            {
                var storedModel = GetJourneyObject<ClientAddPreviousNameViewModel>();
                if (storedModel != null)
                {
                    model = storedModel;
                    TryValidateModel(model);
                }
            }

            model.ReturnAction = returnAction ?? nameof(ClientNamesSummary);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClientAddPreviousName([FromForm] ClientAddPreviousNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(ClientAddPreviousName), new
                {
                    returnAction = model.ReturnAction
                });
            }

            if (model.PreviousNameId == default)
            {
                var id = await _integrationGateway.AddClientPreviousNameAsync(model, GetMoratoriumId());
                model.PreviousNameId = id;
            }
            else
            {
                await _integrationGateway.UpdateClientPreviousNameAsync(model, GetMoratoriumId());
            }

            SetJourneyObject(model);

            return CompleteSubJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public IActionResult DebtorPostcode(
            [FromQuery] bool clear = false,
            [FromQuery] Guid addId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool hasError = false)
        {
            return AddressSearch<AddressSearchViewModel>(
                clear,
                nameof(DebtorPostcode),
                addId,
                returnAction: returnAction ?? nameof(DebtorAddresses),
                hasError: hasError
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorPostcode([FromForm] PostcodeSearchViewModel model)
            => await AddressSearch<AddressSearchViewModel>(model);

        [HttpPost]
        public async Task<IActionResult> DebtorPostcodeSave([FromForm] AddressSearchViewModel model)
        {
            return await DebtorAddressSaveAsync(
                model,
                redirectFaiure: nameof(DebtorPostcode),
                redirectSuccess: model.ReturnAction,
                x => x.AddressId = model.AddressId);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorManualAddressSubmit(
            [FromQuery] Guid addId = default,
            [FromQuery] string returnAction = null)
        {
            var model = new AddressWithValidation();

            if (addId != default)
            {
                DebtorDetailViewModel debtorDetail;

                if (TempData[_debtDetailViewModel] != null)
                {
                    debtorDetail = JsonSerializer.Deserialize<DebtorDetailViewModel>(TempData.Peek(_debtorDetailViewModel).ToString());
                }
                else
                {
                    debtorDetail = await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());
                }

                var address = debtorDetail.AddressDetail.CurrentAddress;

                model.AddressId = address?.AddressId != null ? address.AddressId : default;
                model.AddressLine1 = address?.AddressLine1;
                model.AddressLine2 = address?.AddressLine2;
                model.TownCity = address?.TownCity;
                model.County = address?.County;
                model.Country = address?.Country ?? null;
                model.Postcode = address?.Postcode;
            }
            else
            {
                var storedModel = GetJourneyObject<AddressWithValidation>(nameof(DebtorPostcode));
                if (storedModel != null)
                {
                    model = storedModel;
                    TryValidateModel(model);
                }
            }

            model.ReturnAction = returnAction ?? nameof(DebtorAddresses);

            GenerateCountryListToViewBag();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorManualAddressSubmit([FromForm] AddressWithValidation model)
        {
            if (!ModelState.IsValid || !HasMoratoriumId())
            {

                GenerateCountryListToViewBag();

                SetJourneyObject(model, nameof(DebtorPostcode));
                return ContinueJourneyRedirect(nameof(DebtorManualAddressSubmit), new { returnAction = model.ReturnAction });
            }

            model.AddressId = await _integrationGateway.CreateDebtorAddressAsync(model, GetMoratoriumId());

            SetJourneyObject(model, nameof(DebtorPostcode));

            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public IActionResult DebtorBusinessDetails(
            [FromQuery] string returnAction = null)
        {
            var model = new DebtorBusinessDetailViewModel();

            var storedModel = GetJourneyObject<DebtorBusinessDetailViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model.IsRadioInline = true;
                model.ReturnAction = returnAction ?? nameof(DebtorConfirmDetails);
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorBusinessDetails([FromForm] DebtorBusinessDetailViewModel model)
        {
            SetJourneyObject(model);

            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(
                    nameof(DebtorBusinessDetails),
                    new
                    {
                        returnAction = model.ReturnAction,
                    });
            }

            if (model.SubmitNow)
            {
                return StartNewSubJourneyRedirect(
                    nameof(DebtorAddBusiness),
                    new
                    {
                        returnAction = model.ReturnAction
                    });
            }

            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorAddBusiness(
            [FromQuery] Guid businessId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool edit = false)
        {
            if (!HasMoratoriumId())
                return ContinueJourneyRedirect(nameof(DebtorBusinessDetails));

            var model = new DebtorAddBusinessViewModel();
            Address debtorCurrentAddres = null;
            DebtorAddressViewModel debtorAddressViewModel = null;

            var storedModel = GetJourneyObject<DebtorAddBusinessViewModel>();

            if (storedModel != null)
            {
                model = storedModel;
                businessId = model.BusinessId;
                if (!edit)
                    TryValidateModel(model);
            }

            if (businessId != default)
            {
                var debtorDetail = TempData[_debtDetailViewModel] != null
                    ? JsonSerializer.Deserialize<DebtorDetailViewModel>(TempData.Peek(_debtorDetailViewModel).ToString())
                    : await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

                var businessModel = debtorDetail.BusinessDetails.FirstOrDefault(b => b.BusinessId == businessId);
                model.BusinessId = businessModel.BusinessId;
                model.BusinessName = businessModel.BusinessName;

                debtorCurrentAddres = businessModel.BusinessAddress;
            }
            else
            {
                debtorAddressViewModel = await _integrationGateway.GetDebtorAddresses(GetMoratoriumId());

                debtorCurrentAddres = debtorAddressViewModel.CurrentAddress;
            }

            model.DebtorCurrentAddress = debtorCurrentAddres;
            model.DisplayHideAddressLabel = debtorAddressViewModel?.AddressHidden ?? false;

            model.IsEdit = edit;
            model.ReturnAction = returnAction ?? nameof(DebtorAddBusinessAddress);

            SetJourneyObject(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorAddBusiness([FromForm] DebtorAddBusinessViewModel model)
        {
            var storedModel = GetJourneyObject<DebtorAddBusinessViewModel>();
            model.DebtorCurrentAddress = storedModel.DebtorCurrentAddress;
            model.MoratoriumId = GetMoratoriumId();
            TempData[_debtorBusinessName] = model.BusinessName;

            var errorRedirectParams = new
            {
                businessId = model.BusinessId,
                returnAction = model.ReturnAction,
                edit = false
            };

            if (!ModelState.IsValid)
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(DebtorAddBusiness), errorRedirectParams);
            }

            if (model.UseCurrentAddress)
            {
                if (model.BusinessId == default)
                {
                    var businessRecordReturn = await _integrationGateway.DebtorAddBusinessAsync(model);
                    model.BusinessId = businessRecordReturn.BusinessId;
                    model.DebtorCurrentAddress.AddressId = businessRecordReturn.AddressId;
                }
                else
                {
                    await _integrationGateway.DebtorUpdateBusinessAsync(model);
                }

                SetJourneyObject(model);

                return model.ReturnAction != nameof(DebtorAddBusinessAddress)
                    ? CompleteSubJourneyRedirect(model.ReturnAction)
                    : CompleteSubJourneyRedirect(nameof(DebtorConfirmDetails));
            }

            SetJourneyObject(model);

            return ContinueJourneyRedirect(
                nameof(DebtorAddBusinessAddress),
                new
                {
                    businessId = model.BusinessId,
                    addId = model.DebtorCurrentAddress.AddressId,
                    returnAction = model.ReturnAction,
                    edit = true
                });
        }

        [HttpGet]
        public IActionResult DebtorAddBusinessAddress(
            [FromQuery] bool clear = false,
            [FromQuery] Guid businessId = default,
            [FromQuery] Guid addId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool hasError = false)
        {
            return AddressSearch<AddressSearchViewModel>(
                clear,
                nameof(DebtorAddBusiness),
                businessId: businessId,
                addId: addId,
                returnAction: returnAction ?? nameof(DebtorConfirmDetails),
                hasError: hasError
            );
        }

        [HttpPost]
        public async Task<IActionResult> DebtorAddBusinessAddress([FromForm] PostcodeSearchViewModel viewModel)
            => await AddressSearch<AddressSearchViewModel>(viewModel);

        [HttpPost]
        public async Task<IActionResult> DebtorSaveBusinessAddress([FromForm] AddressSearchViewModel model)
        {
            return await DebtorAddressSaveAsync(
                model,
                nameof(DebtorAddBusinessAddress),
                model.ReturnAction,
                saveAddress: async x =>
                {
                    x.AddressId = model.AddressId;

                    var viewModel = new DebtorAddBusinessViewModel
                    {
                        BusinessId = model.BusinessId,
                        DebtorCurrentAddress = x,
                        BusinessName = TempData.Peek(_debtorBusinessName).ToString(),
                        MoratoriumId = GetMoratoriumId(),
                    };

                    if (viewModel.BusinessId == default)
                    {
                        var result = await _integrationGateway.DebtorAddBusinessAsync(viewModel);
                        return result.AddressId;
                    }
                    else
                    {
                        await _integrationGateway.DebtorUpdateBusinessAsync(viewModel);
                        return model.AddressId;
                    }
                }
            );
        }

        [HttpGet]
        public async Task<IActionResult> DebtorAddBusinessAddressManual(
            [FromQuery] Guid businessId = default,
            [FromQuery] string returnAction = null)
        {
            GenerateCountryListToViewBag();

            var model = new DebtorAddBusinessAddressManualViewModel();
            var storedModel = GetJourneyObject<DebtorAddBusinessAddressManualViewModel>(nameof(DebtorAddBusinessAddress));

            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                var debtorDetail = TempData[_debtDetailViewModel] != null
                    ? JsonSerializer.Deserialize<DebtorDetailViewModel>(TempData.Peek(_debtorDetailViewModel).ToString())
                    : await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

                var businessModel = debtorDetail.BusinessDetails.FirstOrDefault(b => b.BusinessId == businessId);

                model.BusinessId = businessModel?.BusinessId ?? default;
                model.AddressId = businessModel?.BusinessAddress.AddressId != null ? (Guid)businessModel?.BusinessAddress.AddressId : default;
                model.AddressLine1 = businessModel?.BusinessAddress.AddressLine1;
                model.AddressLine2 = businessModel?.BusinessAddress.AddressLine2;
                model.TownCity = businessModel?.BusinessAddress.TownCity;
                model.County = businessModel?.BusinessAddress.County;
                model.Country = businessModel?.BusinessAddress.Country;
                model.Postcode = businessModel?.BusinessAddress.Postcode;
            }

            model.BusinessName = TempData.Peek(_debtorBusinessName).ToString();
            model.ReturnAction = returnAction ?? nameof(DebtorConfirmDetails);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorAddBusinessAddressManual([FromForm] DebtorAddBusinessAddressManualViewModel model)
        {
            if (!ModelState.IsValid || !HasMoratoriumId())
            {
                GenerateCountryListToViewBag();
                SetJourneyObject(model, nameof(DebtorAddBusinessAddress));
                return ContinueJourneyRedirect(nameof(DebtorAddBusinessAddressManual), new
                {
                    businessId = model.BusinessId,
                    returnAction = model.ReturnAction,
                });
            }

            var viewModel = new DebtorAddBusinessViewModel
            {
                DebtorCurrentAddress = model,
                BusinessName = model.BusinessName,
                MoratoriumId = GetMoratoriumId(),
                BusinessId = model.BusinessId
            };

            if (viewModel.BusinessId == default)
                model.AddressId = (await _integrationGateway.DebtorAddBusinessAsync(viewModel)).AddressId;
            else
                await _integrationGateway.DebtorUpdateBusinessAsync(viewModel);

            SetJourneyObject(model, nameof(DebtorAddBusinessAddress));

            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorConfirmDetails()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

            TempData[_debtorDetailViewModel] = JsonSerializer.Serialize(viewModel);


            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorContactPreference(
            [FromQuery] bool edit = false,
            [FromQuery] string returnAction = null)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var model = new DebtorContactPreferenceViewModel();

            if (edit)
            {
                DebtorDetailViewModel debtorDetail = null;
                if (TempData[_debtDetailViewModel] != null)
                    debtorDetail = JsonSerializer.Deserialize<DebtorDetailViewModel>(TempData.Peek(_debtorDetailViewModel).ToString());
                else
                    debtorDetail = await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

                model.SubmitOption = debtorDetail?.NotificationDetail.PreferenceType;
                model.EmailAddress = debtorDetail?.NotificationDetail.EmailAddress;
                model.ConfirmEmailAddress = debtorDetail?.NotificationDetail.EmailAddress;
            }
            else
            {
                var storedModel = GetJourneyObject<DebtorContactPreferenceViewModel>();
                if (storedModel != null)
                {
                    model = storedModel;
                    TryValidateModel(model);
                }
            }

            model.ReturnAction = returnAction ?? nameof(DebtorBusinessDetails);


            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DebtorContactPreference(
            [FromForm] DebtorContactPreferenceViewModel model)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            SetJourneyObject(model);

            if (!ModelState.IsValid)
                return ContinueJourneyRedirect(nameof(DebtorContactPreference), new { returnAction = model.ReturnAction });

            await _integrationGateway.DebtorSetContactPreference(GetMoratoriumId(), model);
            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public IActionResult CreditorSearch(
            [FromQuery] bool fromCreateBreathingSpace,
            [FromQuery] Guid debtId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool edit = false)
        {
            HttpContext.Session.SetString(_debtorAddDebtFromCreateBreathingSpace, fromCreateBreathingSpace.ToString());

            var model = new CreditorSearchViewModel
            {
                DebtId = debtId,
                ReturnAction = returnAction ?? nameof(DebtDetails),
                IsEdit = edit
            };

            var storedModel = GetJourneyObject<CreditorSearchViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreditorSearch(CreditorSearchViewModel modelIn)
        {
            SetJourneyObject(modelIn);

            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(CreditorSearch));
            }

            var modelOut = await _integrationGateway.CmpCreditorSearch(HttpUtility.UrlDecode(modelIn.CreditorName));

            if (modelOut.Creditors != null && modelOut.Creditors.Count > 1)
            {
                SetJourneyObject(modelOut, nameof(CreditorSearchResults));
                return ContinueJourneyRedirect(nameof(CreditorSearchResults));
            }
            else
            {
                return await CreateAdHocCreditor(
                        modelIn.CreditorName,
                        modelIn.DebtId,
                        modelIn.ReturnAction,
                        modelIn.IsEdit
                        );
            }
        }

        [HttpGet("ajax/creditor/new")]
        public Task<IActionResult> CreditorNewAdHocCreditor(
            [FromQuery] string name,
            [FromQuery] Guid debtId,
            [FromQuery] string returnAction,
            [FromQuery] bool edit,
            [FromQuery] bool isDebtTransferred = false) => CreateAdHocCreditor(name, debtId, returnAction, edit, isDebtTransferred);

        [HttpGet("ajax/creditor/search")]
        public async Task<IActionResult> CreditorSearchAjax([FromQuery] string query)
        {
            var searchResults = await _integrationGateway.CmpCreditorSearchAjax(HttpUtility.UrlDecode(query));

            return Json(searchResults);
        }

        [HttpGet("ajax/creditor/submit")]
        public IActionResult CreditorTypeaheadBypass(
            [FromQuery] string id,
            [FromQuery] Guid debtId,
            [FromQuery] string returnAction = null,
            [FromQuery] bool edit = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                ContinueJourneyRedirect(nameof(CreditorSearch), new { debtId, returnAction, edit });

            TempData.Clear();
            HttpContext.Session.SetString(_creditorIdKey, id);

            return ContinueJourneyRedirect(nameof(CreditorAddCmpDebt), new { debtId, returnAction });
        }

        [HttpGet]
        public IActionResult CreditorSearchResults([FromQuery] bool error = false)
        {
            var model = GetJourneyObject<CreditorSearchResultsViewModel>();

            if (error)
                TryValidateModel(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult CreditorSearchResults(CreditorSearchResultsViewModel model)
        {
            var storedModel = GetJourneyObject<CreditorSearchResultsViewModel>();
            model.Creditors = storedModel.Creditors;
            SetJourneyObject(model);

            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(CreditorSearchResults), new { error = true });
            }

            HttpContext.Session.SetString(_creditorIdKey, model.SelectedCreditor.ToString());

            return ContinueJourneyRedirect(nameof(CreditorAddCmpDebt), new { debtId = model.DebtId, returnAction = model.ReturnAction });
        }

        [HttpGet]
        public async Task<IActionResult> CreditorAddCmpDebt(
            [FromQuery] Guid debtId = default,
            [FromQuery] string returnAction = null)
        {
            if (!HttpContext.Session.Keys.Contains(_creditorIdKey))
                return ContinueJourneyRedirect(nameof(CreditorSearch), new { debtId, returnAction });

            var creditorId = HttpContext.Session.GetString(_creditorIdKey);
            var creditor = await _integrationGateway.GetGenericCreditorByIdAsync(creditorId);

            var viewModel = new CreditorAddCmpDebtViewModel
            {
                Id = debtId,
                CreditorName = creditor.Name,
                CreditorId = creditor.Id,
                DebtTypes = creditor.DebtTypes,
                IsGovernmentCreditor = creditor.IsGovermentCreditor,
                ReturnAction = returnAction
            };

            HttpContext.Session.SetString(_creditorKey, JsonSerializer.Serialize(creditor));

            var storedModel = GetJourneyObject<CreditorAddCmpDebtViewModel>();
            if (storedModel != null && storedModel.CreditorId.ToString() == creditorId)
            {
                viewModel = storedModel;
                TryValidateModel(viewModel);
            }

            TempData["debtTypes"] = JsonSerializer.Serialize(viewModel.DebtTypes);


            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreditorAddCmpDebt(CreditorAddCmpDebtViewModel model)
        {
            var debtTypes = JsonSerializer.Deserialize<List<DebtType>>(TempData.Peek("debtTypes").ToString());
            model.DebtTypes = debtTypes;

            if (!ModelState.IsValid)
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(CreditorAddCmpDebt), new
                {
                    debtId = model.Id,
                    returnAction = model.ReturnAction
                });
            }

            if (!HasMoratoriumId())
                return RedirectToHome();

            model.SetSelectedDebtTypeName();
            model.MoratoriumId = GetMoratoriumId();

            if (model.Id == default)
                model.Id = await _integrationGateway.CreatDebtAsync(model);
            else
                await _integrationGateway.UpdateDebtAsync(model);

            HttpContext.Session.SetString(_debtorAddNonCmpDebt, false.ToString());
            HttpContext.Session.SetString(_debtKey, JsonSerializer.Serialize<DebtViewModel>(model));
            SetJourneyObject(model);

            return ContinueJourneyRedirect(model.ReturnAction ?? nameof(DebtDetails));
        }

        [HttpGet]
        public async Task<IActionResult> CreditorAddAdHocDebt(
            [FromQuery] Guid debtId = default,
            [FromQuery] string returnAction = null)
        {
            if (!HttpContext.Session.Keys.Contains(_creditorIdKey))
                return ContinueJourneyRedirect(nameof(CreditorSearch), new { debtId, returnAction });

            var creditorId = HttpContext.Session.GetString(_creditorIdKey);
            var creditor = await _integrationGateway.GetGenericCreditorByIdAsync(creditorId);

            HttpContext.Session.SetString(_creditorKey, JsonSerializer.Serialize(creditor));

            var model = new CreditorAddAdHocDebtViewModel
            {
                CreditorId = creditor.Id,
                CreditorName = creditor.Name,
                Id = debtId,
                ReturnAction = returnAction
            };

            var storedModel = GetJourneyObject<CreditorAddAdHocDebtViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }



            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreditorAddAdHocDebt(CreditorAddAdHocDebtViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(CreditorAddAdHocDebt), new
                {
                    debtId = model.Id,
                    returnAction = model.ReturnAction
                });
            }

            if (!HasMoratoriumId())
                return RedirectToHome();

            model.MoratoriumId = GetMoratoriumId();

            if (model.Id == default)
                model.Id = await _integrationGateway.CreatDebtAsync(model);
            else
                await _integrationGateway.UpdateDebtAsync(model);

            HttpContext.Session.SetString(_debtorAddNonCmpDebt, true.ToString());
            HttpContext.Session.SetString(_debtKey, JsonSerializer.Serialize<DebtViewModel>(model));
            SetJourneyObject(model);

            return ContinueJourneyRedirect(model.ReturnAction ?? nameof(DebtDetails));
        }

        [HttpGet]
        public IActionResult DebtDetails()
        {
            var creditor = JsonSerializer.Deserialize<CreditorResponse>(HttpContext.Session.GetString(_creditorKey));
            var debt = JsonSerializer.Deserialize<DebtViewModel>(HttpContext.Session.GetString(_debtKey));
            var fromCreateBreathingSpace = bool.Parse(HttpContext.Session.GetString(_debtorAddDebtFromCreateBreathingSpace));
            var isNonCmpDebt = bool.Parse(HttpContext.Session.GetString(_debtorAddNonCmpDebt));
            var model = new DebtDetailViewModel { Creditor = creditor, Debt = debt, FromCreateBreathingSpace = fromCreateBreathingSpace, IsNonCmpDebt = isNonCmpDebt, };

            if (!fromCreateBreathingSpace)
            {
                _bannerService.ShowBanner(BannerTexts.NewDebtAddedBanner);
            }



            return View(model);
        }

        [HttpGet]
        public IActionResult DebtorAccountSave(bool submit)
        {
            if (!submit)
            {
                ClearSession();
                return RedirectToHome();
            }

            var model = new DebtorRadioYesNoViewModel();

            var storedModel = GetJourneyObject<DebtorBusinessDetailViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }

            model.YesHint = "Your client will be submitted into Breathing Space";
            model.NoHint = "Your client’s data will be saved, and you can submit it at a later date";

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorAccountSave([FromForm] DebtorRadioYesNoViewModel model)
        {
            SetJourneyObject(model);
            if (!ModelState.IsValid)
                return ContinueJourneyRedirect(nameof(DebtorAccountSave));

            if (model.SubmitOption == "No")
                return RedirectToHome();

            return ContinueJourneyRedirect(nameof(DebtorBreathingSpaceType));
        }

        [HttpGet]
        public IActionResult DebtorBreathingSpaceType()
        {
            var storedModel = GetJourneyObject<DebtorBreathingSpaceTypeViewModel>();
            if (storedModel != null)
                TryValidateModel(storedModel);

            return View(storedModel);
        }

        [HttpPost]
        public async Task<IActionResult> DebtorBreathingSpaceType([FromForm] DebtorBreathingSpaceTypeViewModel model)
        {
            SetJourneyObject(model);
            if (!ModelState.IsValid || !HasMoratoriumId())
                return ContinueJourneyRedirect(nameof(DebtorBreathingSpaceType));

            if (model.SubmitOption == BreathingSpaceType.MentalHealth.ToString())
            {
                await _integrationGateway.SetBreathingSpaceAsMentalHealth(GetMoratoriumId());
                return ContinueJourneyRedirect(nameof(DebtorEligibilityConfirmation));
            }
            else
            {
                await _integrationGateway.SetBreathingSpaceAsStandard(GetMoratoriumId());
            }

            return ContinueJourneyRedirect(nameof(DebtorDeclaration));
        }

        [HttpGet]
        public async Task<IActionResult> DebtorDeclaration()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());


            return View(viewModel.PersonalDetail);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorAccountConfirmation()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var submitViewModel = new DebtorRadioYesNoViewModel { MoratoriumId = GetMoratoriumId() };

            var viewModel = await _integrationGateway.SubmitDebtorAccount(submitViewModel);

            ClearSession();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorHideAddress([FromQuery] string returnAction = null)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var result = await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

            var model = new DebtorHideAddressViewModel();
            var storedModel = GetJourneyObject<DebtorHideAddressViewModel>();

            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model.ReturnAction = returnAction ?? nameof(DebtorAddresses);
            }

            var breathingSpaceHiddenAddressKey = $"{_addressHiddenKey}{GetMoratoriumId()}";
            var breathingSpaceAddressHidden = HasSessionKey(breathingSpaceHiddenAddressKey) ? GetSessionObject<bool>(breathingSpaceHiddenAddressKey) : false;
            var shouldPopulateModel = result.AddressDetail.AddressHidden || breathingSpaceAddressHidden;
            if (shouldPopulateModel)
            {
                model.IsYesChecked = result.AddressDetail.AddressHidden;
                model.IsNoChecked = !result.AddressDetail.AddressHidden;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorHideAddress(DebtorHideAddressViewModel model)
        {
            if (!ModelState.IsValid || !HasMoratoriumId())
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(DebtorHideAddress), new { returnAction = model.ReturnAction });
            }

            model.MoratoriumId = GetMoratoriumId();

            await _integrationGateway.DebtorHideAddressAsync(model);

            var breathingSpaceHiddenAddressKey = $"{_addressHiddenKey}{model.MoratoriumId}";
            SetSessionObject(breathingSpaceHiddenAddressKey, true);

            SetJourneyObject(model);
            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public IActionResult DebtorEligibilityConfirmation() => View(new DebtorEligibilityViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorEligibilityConfirmation([FromForm] DebtorEligibilityViewModel viewModel) => ContinueJourneyRedirect(nameof(DebtorAddNominatedContact));

        [HttpGet]
        public async Task<IActionResult> DebtorAddNominatedContact(
            [FromQuery] bool edit = false,
            [FromQuery] string inputFocus = null,
            [FromQuery] bool addressChange = false,
            [FromQuery] string returnAction = null)
        {
            var model = new DebtorNominatedContactViewModel()
            {
                ReturnAction = returnAction ?? nameof(DebtorPointContactSummary),
                IsEdit = edit
            };

            var storedModel = GetJourneyObject<DebtorNominatedContactViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else if (edit)
            {
                var viewModel = await _integrationGateway.GetDebtorNominatedContactSummary(GetMoratoriumId());
                model.ContactId = viewModel.ContactId;
                model.RoleId = viewModel.RoleId;
                model.PointContactRole = viewModel.PointContactRole.ToString();
                model.FullName = viewModel.FullName;
                model.TelephoneNumber = viewModel.TelephoneNumber;
                model.EmailAddress = viewModel.EmailAddress;
                model.ConfirmEmailAddress = viewModel.EmailAddress;
                model.ContactConfirmationMethod = viewModel.NotificationMethod.ToString();
                model.CommunicationAddress = viewModel.CommunicationAddress;
                model.InputFocus = inputFocus;
                model.IsAddressChangeRequested = addressChange;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorAddNominatedContact([FromForm] DebtorNominatedContactViewModel model)
        {
            if (!ModelState.IsValid || !HasMoratoriumId())
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(DebtorAddNominatedContact), new
                {
                    addressChange = model.IsAddressChangeRequested,
                    returnAction = model.ReturnAction,
                });
            }

            model.MoratoriumId = GetMoratoriumId();

            if (model.AllowAddressChange)
            {
                SetSessionObject(_nominatedContactViewModel, model);
                return ContinueJourneyRedirect(nameof(DebtorPointContactAddress),
                    new
                    {
                        addId = model.CommunicationAddress?.AddressId ?? default,
                        returnAction = model.ReturnAction,
                        clear = true
                    });
            }

            if (model.ConditionalFlag && model.CommunicationAddress.AddressId == default)
                model.CommunicationAddress = null;

            if (model.ContactId == default && model.RoleId == default)
            {
                model.ContactId = (await _integrationGateway.CreateNominatedContactAsync(model)).ContactId;
            }
            else
            {
                await _integrationGateway.UpdateNominatedContactAsync(model);
            }

            SetJourneyObject(model);
            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public IActionResult DebtorPointContactAddress(
            [FromQuery] bool clear = false,
            [FromQuery] Guid addId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool hasError = false)
        {
            return AddressSearch<AddressSearchViewModel>(
                clear,
                nameof(DebtorPointContactAddress),
                addId: addId,
                returnAction: returnAction,
                hasError: hasError
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorPointContactAddress([FromForm] PostcodeSearchViewModel model)
                => await AddressSearch<AddressSearchViewModel>(model);

        [HttpPost]
        public async Task<IActionResult> DebtorPointContactAddressSave([FromForm] AddressSearchViewModel model)
        {
            return await DebtorAddressSaveAsync(
                model,
                redirectFaiure: nameof(DebtorPointContactAddress),
                redirectSuccess: model.ReturnAction,
                saveAddress: async model =>
                {
                    var storedModel = GetSessionObject<DebtorNominatedContactViewModel>(_nominatedContactViewModel);
                    model.AddressId = storedModel.CommunicationAddress.AddressId;
                    storedModel.MoratoriumId = GetMoratoriumId();
                    storedModel.CommunicationAddress = model;

                    if (storedModel.ContactId == default)
                    {
                        var result = await _integrationGateway.CreateNominatedContactAsync(storedModel);
                        return result.AddressId;
                    }
                    else
                    {
                        await _integrationGateway.UpdateNominatedContactAsync(storedModel);
                        return model.AddressId;
                    }
                }
            );
        }

        [HttpGet]
        public IActionResult DebtorPointContactAddressManual(
            )
        {
            var model = new AddressWithValidation();

            var storedModel = GetJourneyObject<AddressWithValidation>(nameof(DebtorPointContactAddress));
            if (storedModel != null)
            {
                model = storedModel;
                model.ReturnAction = nameof(DebtorPointContactSummary);
                TryValidateModel(model);
            }
            else
            {
                if (HasSessionKey(_nominatedContactViewModel))
                {
                    var viewModel = GetSessionObject<DebtorNominatedContactViewModel>(_nominatedContactViewModel);
                    var address = viewModel.CommunicationAddress;

                    model.AddressId = address?.AddressId ?? default;
                    model.AddressLine1 = address?.AddressLine1;
                    model.AddressLine2 = address?.AddressLine2;
                    model.TownCity = address?.TownCity;
                    model.Postcode = address?.Postcode;
                    model.County = address?.County;
                    model.Country = address?.Country;
                    model.ReturnAction = viewModel?.ReturnAction;
                }
            }
            GenerateCountryListToViewBag();


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorPointContactAddressManual([FromForm] AddressWithValidation model)
        {
            if (!ModelState.IsValid || !HasMoratoriumId())
            {
                GenerateCountryListToViewBag();
                SetJourneyObject(model, nameof(DebtorPointContactAddress));
                return ContinueJourneyRedirect(nameof(DebtorPointContactAddressManual));
            }

            var storedModel = GetSessionObject<DebtorNominatedContactViewModel>(_nominatedContactViewModel);
            model.AddressId = storedModel.CommunicationAddress.AddressId;
            storedModel.MoratoriumId = GetMoratoriumId();
            storedModel.CommunicationAddress = model;

            if (storedModel.ContactId == default)
            {
                var result = await _integrationGateway.CreateNominatedContactAsync(storedModel);
                model.AddressId = result.AddressId;
            }
            else
            {
                await _integrationGateway.UpdateNominatedContactAsync(storedModel);
            }

            SetJourneyObject(model, nameof(DebtorPointContactAddress));

            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorPointContactSummary()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = await _integrationGateway.GetDebtorNominatedContactSummary(GetMoratoriumId());

            var nominatedContactViewModel = viewModel.ToNominatedContactViewModel();
            nominatedContactViewModel.ReturnAction = nameof(DebtorPointContactSummary);
            SetSessionObject(_nominatedContactViewModel, nominatedContactViewModel);

            return View(viewModel);
        }

        #endregion

        #region Debtor Account Search

        [HttpGet]
        public IActionResult AccountSearch(string dateValidationMessage = "")
        {
            var model = new AccountSearchViewModel { IsValidDateOfBirth = true };

            var storedModel = GetJourneyObject<AccountSearchViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }

            ViewData[_dateValidationMessage] = dateValidationMessage;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AccountSearch(AccountSearchViewModel model)
        {
            SetJourneyObject(model);

            if (!ModelState.IsValid)
                return ContinueJourneyRedirect(nameof(AccountSearch), new { dateValidationMessage = "Invalid Date of birth" });

            return ContinueJourneyRedirect(nameof(AccountSearchResult));
        }

        [HttpGet]
        public async Task<IActionResult> AccountSearchResult()
        {
            var model = GetJourneyObject<AccountSearchViewModel>(nameof(AccountSearch));

            await _integrationGateway.SearchAccounts(model);

            return View(model);
        }

        [HttpGet]
        public IActionResult AccountSearchNavigate([FromQuery] string moratoriumId)
        {
            SetMoratoriumId(moratoriumId);
            GenerateNewJourneyId();
            return ContinueJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public async Task<IActionResult> DebtorAccountSummary()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            if (TempData[_bsSummaryViewModel] != null)
                TempData.Remove(_bsSummaryViewModel);

            var viewModel = await GetAccountSummary();

            ViewData["IsBreathingSpaceEnded"] = viewModel.DebtorDetail.PersonalDetail.IsEnded;

            SetTempDataObject(_bsSummaryViewModel, viewModel);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorAddresses([FromQuery] string returnAction)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = await _integrationGateway.GetDebtorAddresses(GetMoratoriumId());
            viewModel.ReturnAction = returnAction ?? nameof(DebtorAddresses);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult DebtorPreviousAddress(
            [FromQuery] bool clear = false,
            [FromQuery] Guid addId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool hasError = false)
        {
            return AddressSearch<DebtorPreviousAddressViewModel>(
                clear,
                nameof(DebtorPreviousAddress),
                addId,
                returnAction: returnAction ?? nameof(DebtorAddresses),
                hasError: hasError
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorPreviousAddress([FromForm] PostcodeSearchViewModel model)
            => await AddressSearch<DebtorPreviousAddressViewModel>(model);

        [HttpGet]
        public async Task<IActionResult> DebtorPreviousAddressManual(
            [FromQuery] Guid addId = default,
            [FromQuery] string returnAction = null)
        {
            var model = new DebtorPreviousAddressSubmitViewModel();

            if (addId != default)
            {
                DebtorDetailViewModel debtorDetail;

                if (TempData[_debtDetailViewModel] != null)
                    debtorDetail = JsonSerializer.Deserialize<DebtorDetailViewModel>(TempData.Peek(_debtorDetailViewModel).ToString());
                else
                    debtorDetail = await _integrationGateway.GetDebtorConfirmDetails(GetMoratoriumId());

                var address = debtorDetail.AddressDetail.PreviousAddresses.FirstOrDefault(a => a.AddressId == addId);

                model.AddressId = address?.AddressId != null ? address.AddressId : default;
                model.AddressLine1 = address?.AddressLine1;
                model.AddressLine2 = address?.AddressLine2;
                model.TownCity = address?.TownCity;
                model.County = address?.County;
                model.Country = address?.Country ?? null;
                model.Postcode = address?.Postcode;
                model.MonthFrom = address?.DateFrom.HasValue ?? false ? address?.DateFrom.Value.Month : null;
                model.YearFrom = address?.DateFrom.HasValue ?? false ? address?.DateFrom.Value.Year : null;
                model.MonthTo = address?.DateTo.HasValue ?? false ? address?.DateTo.Value.Month : null;
                model.YearTo = address?.DateTo.HasValue ?? false ? address?.DateTo.Value.Year : null;
            }
            else
            {
                var storedModel = GetJourneyObject<DebtorPreviousAddressSubmitViewModel>(nameof(DebtorPreviousAddress));
                if (storedModel != null)
                {
                    model = storedModel;
                    TryValidateModel(model);
                }
            }

            model.ReturnAction = returnAction ?? nameof(DebtorAddresses);

            GenerateCountryListToViewBag();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorPreviousAddressManual([FromForm] DebtorPreviousAddressSubmitViewModel model)
        {
            if (!ModelState.IsValid || !HasMoratoriumId())
            {

                GenerateCountryListToViewBag();

                SetJourneyObject(model, nameof(DebtorPreviousAddress));

                return ContinueJourneyRedirect(nameof(DebtorPreviousAddressManual), new
                {
                    returnAction = model.ReturnAction
                });
            }

            model.SetFromToDate();

            model.AddressId = await _integrationGateway.CreateDebtorPreviousAddressAsync(model, GetMoratoriumId());

            SetJourneyObject(model, nameof(DebtorPreviousAddress));

            return ContinueJourneyRedirect(model.ReturnAction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorPreviousAddressSubmit([FromForm] DebtorPreviousAddressViewModel model)
        {
            return await DebtorAddressSaveAsync(
                model,
                nameof(DebtorPreviousAddress),
                model.ReturnAction,
                x =>
                {
                    x.AddressId = model.AddressId;
                    x.DateFrom = model.MoveInDate;
                    x.DateTo = model.MoveOutDate;
                },
                async address => await _integrationGateway.CreateDebtorPreviousAddressAsync(address, GetMoratoriumId()));
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> DebtorAccountDetails()
        {

            if (!HasMoratoriumId())
                return RedirectToHome();

            if (TempData[_debtDetailViewModel] != null)
                TempData.Remove(_debtDetailViewModel);

            if (TempData[_bsSummaryViewModel] != null)
                TempData.Remove(_bsSummaryViewModel);

            if (HasSessionKey(_nominatedContactViewModel))
                RemoveSessionByKey(_nominatedContactViewModel);

            var viewModel = await GetAccountSummary();

            ViewData["IsBreathingSpaceEnded"] = viewModel.DebtorDetail.PersonalDetail.IsEnded;

            SetTempDataObject(_bsSummaryViewModel, viewModel);

            if (!HasSessionKey(_nominatedContactViewModel))
            {
                var nominatedContactViewModel = viewModel.DebtorDetail.DebtorNominatedContactSummary?.ToNominatedContactViewModel();

                if (nominatedContactViewModel != null)
                {
                    nominatedContactViewModel.ReturnAction = nameof(DebtorAccountDetails);
                    nominatedContactViewModel.MoratoriumId = GetMoratoriumId();

                    SetSessionObject(_nominatedContactViewModel, nominatedContactViewModel);
                }
            }

            if (TempData[_confirmProposedDebtReview] != null)
            {
                var proposedDebtViewModel = GetTempDataObject<DebtorProposedDebtViewModel>(_confirmProposedDebtReview);
                if (proposedDebtViewModel.AcceptProposedDebt.Value)
                {
                    viewModel.AcceptedProposedDebtId = proposedDebtViewModel.DebtDetailViewModel.Debt.Id;
                }
            }

            if (!viewModel.IsOwningOrganization)
            {
                if (viewModel.DebtorDetail.PersonalDetail.IsEnded)
                {
                    return View(_debtorNotOwningEndedAccountDetails, viewModel);
                }

                return View(_debtorNotOwningAccountDetails, viewModel);
            }
            if (viewModel.DebtorDetail.PersonalDetail.IsEnded)
            {
                return View(_debtorEndedAccountDetails, viewModel);
            }
            if (!viewModel.DebtorDetail.PersonalDetail.IsActive)
            {
                return View(_debtorInactiveAccountDetails, viewModel);
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DebtEligibilityReview([FromQuery] Guid debtId)
        {
            var model = new DebtEligibilityReviewSummaryViewModel();
            var storedModel = GetJourneyObject<DebtEligibilityReviewSummaryViewModel>();

            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                var debtDetails = await GetDebtDetails(debtId);

                model.DebtDetailViewModel = debtDetails;
            }

            TempData[_debtDetailViewModel] = JsonSerializer.Serialize(model.DebtDetailViewModel);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtEligibilityReview(DebtEligibilityReviewSummaryViewModel model)
        {
            model.DebtDetailViewModel = JsonSerializer.Deserialize<DebtDetailViewModel>(
                TempData.Peek(_debtDetailViewModel).ToString()
            );

            SetJourneyObject(model);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtEligibilityReviewConfirmation))
                : ContinueJourneyRedirect(nameof(DebtEligibilityReview));
        }

        [HttpGet]
        public IActionResult DebtEligibilityReviewConfirmation()
        {
            var model = GetJourneyObject<DebtEligibilityReviewSummaryViewModel>(nameof(DebtEligibilityReview));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DebtEligibilityReviewSubmission()
        {
            var model = GetJourneyObject<DebtEligibilityReviewSummaryViewModel>(nameof(DebtEligibilityReview));

            if (await _integrationGateway.SubmitDebtEligibilityReview(model) != default)
            {
                model.HasReviewSubmitted = true;

                if (model.HasDebtRemoved)
                    _bannerService.ShowBanner(BannerTexts.DebtRemovedAfterEligibilityReviewBanner);
                else
                    _bannerService.ShowBanner(BannerTexts.DebtNotRemovedAfterEligibilityReviewBanner);
            }

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public IActionResult DebtorEligibilityReview([FromQuery] Guid creditorId)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var model = new DebtorEligibilityReviewDecisionViewModel();
            var storedModel = GetJourneyObject<DebtorEligibilityReviewDecisionViewModel>();

            if (storedModel is null)
            {
                var summaryModel = GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel);

                var debtorReviewModel = summaryModel.DebtDetails
                    .Select(d => d?.DebtorClientEligibilityReview)
                    .FirstOrDefault(d => d?.CreditorId == creditorId);

                model = new DebtorEligibilityReviewDecisionViewModel(debtorReviewModel)
                {
                    MoneyAdviserName = "Paul Smith",
                    MoneyAdviserOrganisation = "Advice UK",
                    CreditorName = "Kiran Callaghan"
                };

                SetJourneyObject(model);
            }
            else
            {
                model = storedModel;
                TryValidateModel(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorEligibilityReview(DebtorEligibilityReviewDecisionViewModel model)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var updatedViewModel = GetJourneyObject<DebtorEligibilityReviewDecisionViewModel>();
            updatedViewModel.EndBreathingSpace = model.EndBreathingSpace;
            updatedViewModel.MoneyAdviserNotes = model.MoneyAdviserNotes;

            SetJourneyObject(updatedViewModel);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtorEligibilityReviewConfirmation))
                : ContinueJourneyRedirect(nameof(DebtorEligibilityReview), new { creditorId = model.CreditorId });
        }

        [HttpGet]
        public IActionResult DebtorEligibilityReviewConfirmation()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = GetJourneyObject<DebtorEligibilityReviewDecisionViewModel>(nameof(DebtorEligibilityReview));

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorConfirmEligibilityReview()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = GetJourneyObject<DebtorEligibilityReviewDecisionViewModel>(nameof(DebtorEligibilityReview));

            var reviewId = await _integrationGateway.DebtorReviewClientEligibility(GetMoratoriumId(), viewModel);

            _bannerService.ShowBanner(BannerTexts.ClientNotRemovedAfterEligibilityReviewBanner);

            if (viewModel.EndBreathingSpace.IsTrue())
            {
                var submitOption = viewModel.DebtorEligibilityReviewParentReason.Value switch
                {
                    BreathingSpaceClientEndReasonType.AbleToPayDebts => BreathingSpaceEndReasonType.AbleToPayDebts,
                    BreathingSpaceClientEndReasonType.NoLongerEligible => BreathingSpaceEndReasonType.NoLongerEligible
                };

                var endBreathingSpaceViewModel = new DebtorAccountEndReasonConfirmationViewModel
                {
                    SubmitOption = submitOption,
                    IsPartOfThirtyDayReview = false,
                    NoLongerEligibleReason = viewModel.DebtorEligibilityReviewChildReason,
                };

                await _integrationGateway.DebtorEndAccount(GetMoratoriumId(), endBreathingSpaceViewModel);

                return ContinueJourneyRedirect(nameof(DebtorAccountEndConfirmation));
            }

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public async Task<IActionResult> DebtorEligibilityReviewTask([FromQuery] Guid debtId)
        {
            var model = new DebtorEligibilityReviewSummaryViewModel();
            var storedModel = GetJourneyObject<DebtorEligibilityReviewSummaryViewModel>();

            _bannerService.ShowBanner(BannerTexts.ClientEligibilityReviewCreatedBanner);
            if (storedModel is null)
            {
                var debtDetails = await GetDebtDetails(debtId);

                model = new DebtorEligibilityReviewSummaryViewModel
                {
                    DebtDetailViewModel = debtDetails,
                };


                SetJourneyObject(model);
            }
            else
            {
                model = storedModel;
                TryValidateModel(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorEligibilityReviewTask(DebtorEligibilityReviewSummaryViewModel model)
        {
            var summaryModel = GetJourneyObject<DebtorEligibilityReviewSummaryViewModel>();

            model.DebtDetailViewModel = summaryModel.DebtDetailViewModel;
            model.CreditorId = summaryModel.DebtDetailViewModel.Creditor.Id;

            SetJourneyObject(model);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtorEligibilityReviewTaskConfirmation))
                : ContinueJourneyRedirect(nameof(DebtorEligibilityReviewTask));
        }

        [HttpGet]
        public IActionResult DebtorEligibilityReviewTaskConfirmation()
        {
            var model = GetJourneyObject<DebtorEligibilityReviewSummaryViewModel>(nameof(DebtorEligibilityReviewTask));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorEligibilityReviewTaskSubmission()
        {
            var model = GetJourneyObject<DebtorEligibilityReviewSummaryViewModel>(nameof(DebtorEligibilityReviewTask));

            model.MoratoriumId = GetMoratoriumId();

            await _integrationGateway.SubmitDebtorEligibilityReviewTask(model);

            _bannerService.ShowBanner(BannerTexts.ClientEligibilityReviewCreatedBanner);

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public async Task<IActionResult> DebtEligibilityReviewTask([FromQuery] Guid debtId)
        {
            DebtElgibilityReviewTaskSummaryViewModel model;
            var storedModel = GetJourneyObject<DebtElgibilityReviewTaskSummaryViewModel>();

            if (storedModel is null)
            {
                var debtDetails = await GetDebtDetails(debtId);

                model = new DebtElgibilityReviewTaskSummaryViewModel
                {
                    DebtDetailViewModel = debtDetails
                };

                SetJourneyObject(model);
            }
            else
            {
                model = storedModel;
                TryValidateModel(model);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtEligibilityReviewTask(DebtElgibilityReviewTaskSummaryViewModel model)
        {
            var summaryModel = GetJourneyObject<DebtElgibilityReviewTaskSummaryViewModel>();
            model.DebtDetailViewModel = summaryModel.DebtDetailViewModel;

            SetJourneyObject(model);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtEligibilityReviewTaskConfirmation))
                : ContinueJourneyRedirect(nameof(DebtEligibilityReviewTask));
        }

        [HttpGet]
        public IActionResult DebtEligibilityReviewTaskConfirmation()
        {
            var model = GetJourneyObject<DebtElgibilityReviewTaskSummaryViewModel>(nameof(DebtEligibilityReviewTask));

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DebtEligibilityReviewTaskSubmission()
        {
            var model = GetJourneyObject<DebtElgibilityReviewTaskSummaryViewModel>(nameof(DebtEligibilityReviewTask));

            await _integrationGateway.SubmitDebtEligibilityReviewTask(model);

            _bannerService.ShowBanner(BannerTexts.DebtEligibilityReviewCreatedBanner);

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public async Task<IActionResult> DebtorProposedDebt([FromQuery] Guid debtId)
        {
            var debtDetailViewModel = await GetDebtDetails(debtId);

            var model = new DebtorProposedDebtViewModel
            {
                DebtDetailViewModel = debtDetailViewModel
            };

            var storedModel = GetJourneyObject<DebtorProposedDebtViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }

            SetJourneyObject(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorProposedDebt(DebtorProposedDebtViewModel model)
        {
            var storedModel = GetJourneyObject<DebtorProposedDebtViewModel>();
            model.DebtDetailViewModel = storedModel.DebtDetailViewModel;
            SetJourneyObject(model);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtorProposedDebtConfirm))
                : ContinueJourneyRedirect(nameof(DebtorProposedDebt), new { debtId = storedModel.DebtDetailViewModel.Debt.Id });
        }

        [HttpGet]
        public IActionResult DebtorProposedDebtConfirm()
        {
            var storedModel = GetJourneyObject<DebtorProposedDebtViewModel>(nameof(DebtorProposedDebt));

            return View(storedModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorConfirmProposedDebt()
        {
            var storedModel = GetJourneyObject<DebtorProposedDebtViewModel>(nameof(DebtorProposedDebt));

            await _integrationGateway.MakeProposedDebtDetermination(new ReviewProposedDebtRequest
            {
                AcceptProposedDebt = storedModel.AcceptProposedDebt,
                DebtId = storedModel.DebtDetailViewModel.Debt.Id,
                RemovalReason = storedModel.RemovalReason,
            });

            SetTempDataObject(_confirmProposedDebtReview, storedModel);

            if (storedModel.AcceptProposedDebt.Value)
                _bannerService.ShowBanner(BannerTexts.ProposedDebtAddedBanner);
            else
                _bannerService.ShowBanner(BannerTexts.ProposedDebtRejectedBanner);

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public async Task<IActionResult> DebtorDebtSoldSearch([FromQuery] Guid debtId)
        {
            var model = new DebtorDebtSoldSearchViewModel();
            var storedModel = GetJourneyObject<DebtorDebtSoldSearchViewModel>();
            if (storedModel is null)
            {
                var debtDetails = await GetDebtDetails(debtId);

                model = new DebtorDebtSoldSearchViewModel
                {
                    Debt = debtDetails.Debt,
                    Creditor = debtDetails.Creditor
                };
                SetTempDataObject(_debtKey, debtDetails.Debt.Id);
            }
            else
            {
                model = storedModel;
                TryValidateModel(model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DebtorDebtSoldSearch(DebtorDebtSoldSearchViewModel modelIn)
        {
            if (!ModelState.IsValid)
            {
                var stashedModel = GetJourneyObject<DebtorDebtSoldSearchViewModel>();
                modelIn.Creditor = stashedModel.Creditor;
                modelIn.Debt = stashedModel.Debt;

                SetJourneyObject(modelIn);

                return ContinueJourneyRedirect(nameof(DebtorDebtSoldSearch));
            }

            var modelOut = new DebtorDebtSoldSearchResultsViewModel(await _integrationGateway.CmpCreditorSearch(HttpUtility.UrlDecode(modelIn.CreditorName)));

            SetJourneyObject(modelOut, nameof(DebtorDebtSoldSearchResults));

            return modelIn.Creditor == null
                   ? await CreateAdHocCreditor(modelIn.CreditorName, modelIn.Debt.Id, isDebtTransferred: true)
                   : ContinueJourneyRedirect(nameof(DebtorDebtSoldSearchResults));
        }

        [HttpGet]
        public IActionResult DebtorDebtSoldSearchResults()
        {
            var model = GetJourneyObject<DebtorDebtSoldSearchResultsViewModel>();
            if (model.HasHadAnError)
                TryValidateModel(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DebtorDebtSoldSearchResults(DebtorDebtSoldSearchResultsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Creditors = GetJourneyObject<List<SelectListItem>>();
                model.HasHadAnError = true;
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(DebtorDebtSoldSearchResults));
            }

            return await SubmitSoldOnDebtAndRedirectToTransfer(model.SelectedCreditor.Value);
        }

        [HttpGet]
        public async Task<IActionResult> DebtorDebtSold([FromRoute] Guid id) => await SubmitSoldOnDebtAndRedirectToTransfer(id);

        [HttpGet]
        public async Task<IActionResult> DebtorTransferDebt([FromQuery] Guid debtId)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var debtDetails = await GetDebtDetails(debtId);

            var model = new DebtTransferConfirmationViewModel
            {
                DebtDetail = debtDetails,
            };

            var isAdHocCreditor = HttpContext.Session.TryGetValue(_creditorNameKey, out _);
            HttpContext.Session.Remove(_creditorNameKey);
            if (isAdHocCreditor)
            {
                var soldToCreditorId = HttpContext.Session.GetString(_creditorIdKey);
                model.IsAdHocCreditor = isAdHocCreditor;
                model.DebtSoldToCreditor = await _integrationGateway.GetGenericCreditorByIdAsync(soldToCreditorId);
            }

            var storedModel = GetJourneyObject<DebtTransferConfirmationViewModel>();
            if (storedModel != null)
                model = storedModel;
            else
                SetJourneyObject(model);

            return View(model);
        }

        [HttpGet]
        public IActionResult DebtorTransferDebtConfirmation()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = GetJourneyObject<DebtTransferConfirmationViewModel>(nameof(DebtorTransferDebt));

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorTransferDebtSubmit()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = GetJourneyObject<DebtTransferConfirmationViewModel>(nameof(DebtorTransferDebt));

            await _integrationGateway.ConfirmDebtSold(viewModel.DebtDetail.Debt.Id);

            SetJourneyObject(viewModel);
            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        #endregion

        #region Debtor End Breathing Space

        [HttpGet]
        public IActionResult DebtorAccountMentalHealthEndReason()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = new DebtorAccountEndReasonViewModel
            {
                IsInMentalHealthMoratorium = true,
            };

            var storedModel = GetJourneyObject<DebtorAccountEndReasonViewModel>();
            if (storedModel != null)
            {
                viewModel = storedModel;
                TryValidateModel(viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorAccountMentalHealthEndReason(DebtorAccountEndReasonViewModel model)
        {
            SetJourneyObject(model);

            if (!ModelState.IsValid)
                return ContinueJourneyRedirect(nameof(DebtorAccountMentalHealthEndReason));

            if (!HasMoratoriumId())
                return RedirectToHome();

            return ContinueJourneyRedirect(nameof(DebtorAccountMentalHealthEndReasonConfirmation));
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult DebtorAccountMentalHealthEndReasonConfirmation()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var previousConfirmationViewModel = GetJourneyObject<DebtorAccountEndReasonConfirmationViewModel>(nameof(DebtorEndBreathingSpace));
            if (previousConfirmationViewModel?.AlreadyConfirmed == true)
            {
                return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
            }

            var model = GetJourneyObject<DebtorAccountEndReasonViewModel>(nameof(DebtorAccountMentalHealthEndReason));

            var confirmViewModel = new DebtorAccountEndReasonConfirmationViewModel
            {
                IsInMentalHealthMoratorium = model.IsInMentalHealthMoratorium,
                SubmitOption = model.SubmitOption.Value,
                IsPartOfThirtyDayReview = model.IsPartOfThirtyDayReviewRequiredAndAnswered(),
                NoLongerEligibleReason = model.NoLongerEligibleReason ?? null,
                ReasonMessage = model.GetBreathingSpaceEndReasonConfirmationMessage(),
                DateOfDeath = model.DateOfDeath,
                EndTreatmentDate = model.TreatmentEndDate
            };

            SetJourneyObject(confirmViewModel, nameof(DebtorEndBreathingSpace));

            return View(confirmViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorEndBreathingSpace()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var model = GetJourneyObject<DebtorAccountEndReasonConfirmationViewModel>();
            if (model.AlreadyConfirmed)
            {
                return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
            }

            await _integrationGateway.DebtorEndAccount(GetMoratoriumId(), model);
            model.AlreadyConfirmed = true;

            SetJourneyObject(model, nameof(DebtorEndBreathingSpace));

            return ContinueJourneyRedirect(nameof(DebtorAccountEndConfirmation));
        }

        [HttpGet]
        public IActionResult DebtorEndStandardBreathingSpaceDecision()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var viewModel = new DebtorAccountEndReasonViewModel
            {
                IsInMentalHealthMoratorium = false,
            };

            var storedModel = GetJourneyObject<DebtorAccountEndReasonViewModel>();
            if (storedModel != null)
            {
                viewModel = storedModel;
                TryValidateModel(viewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorEndStandardBreathingSpaceDecision(DebtorAccountEndReasonViewModel model)
        {
            SetJourneyObject(model);

            if (!ModelState.IsValid)
                return ContinueJourneyRedirect(nameof(DebtorEndStandardBreathingSpaceDecision));

            if (!HasMoratoriumId())
                return RedirectToHome();

            return ContinueJourneyRedirect(nameof(DebtorAccountStandardEndBreathingSpaceConfirmation));
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult DebtorAccountStandardEndBreathingSpaceConfirmation()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var previousConfirmationViewModel = GetJourneyObject<DebtorAccountEndReasonConfirmationViewModel>(nameof(DebtorEndBreathingSpace));
            if (previousConfirmationViewModel?.AlreadyConfirmed == true)
            {
                return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
            }

            var model = GetJourneyObject<DebtorAccountEndReasonViewModel>(nameof(DebtorEndStandardBreathingSpaceDecision));

            var confirmViewModel = new DebtorAccountEndReasonConfirmationViewModel
            {
                IsInMentalHealthMoratorium = model.IsInMentalHealthMoratorium,
                SubmitOption = model.SubmitOption.Value,
                IsPartOfThirtyDayReview = model.IsPartOfThirtyDayReviewRequiredAndAnswered(),
                NoLongerEligibleReason = model.NoLongerEligibleReason ?? null,
                ReasonMessage = model.GetBreathingSpaceEndReasonConfirmationMessage(),
                DateOfDeath = model.DateOfDeath,
                EndTreatmentDate = model.TreatmentEndDate
            };

            SetJourneyObject(confirmViewModel, nameof(DebtorEndBreathingSpace));

            return View(confirmViewModel);
        }

        [HttpGet]
        public IActionResult DebtorAccountEndConfirmation() => View();

        #endregion

        #region Creditor Details

        [HttpGet]
        public IActionResult CreditorPostcode(
            [FromQuery] string name,
            [FromQuery] bool clear = false,
            [FromQuery] Guid debtId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool hasError = false,
            [FromQuery] bool edit = false)
        {
            SetAdHocCreditorNameToViewBag();

            return AddressSearch<AddressSearchViewModel>(
                clear,
                nameof(CreditorPostcode),
                debtId: debtId,
                returnAction: returnAction,
                hasError: hasError
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreditorPostcode(PostcodeSearchViewModel model)
        {
            SetAdHocCreditorNameToViewBag();

            return await AddressSearch<AddressSearchViewModel>(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreditorPostcodeSave(AddressSearchViewModel model)
        {
            SetJourneyObject(model);

            if (!ModelState.IsValid || !HttpContext.Session.Keys.Contains(_creditorIdKey))
            {
                SetAdHocCreditorNameToViewBag();
                var storedModel = GetJourneyObject<AddressSearchViewModel>(nameof(CreditorPostcode));
                model.Addresses = storedModel.Addresses;
                return ContinueJourneyRedirect(nameof(CreditorPostcode), new { debtId = model.DebtId, hasError = true });
            }

            var creditorId = new Guid(HttpContext.Session.GetString(_creditorIdKey));
            var address = await _addressLookupService.GetFullAddressAsync(model.SelectedAddress);

            model.AddressId = await _integrationGateway.AddAdHocCreditorAddressAsync(address, creditorId);
            SetJourneyObject(model, nameof(CreditorPostcode));

            bool.TryParse(HttpContext.Session.GetString(_isDebtTransferred), out var isDebtTransferred);
            if (isDebtTransferred)
            {
                return await SubmitSoldOnDebtAndRedirectToTransfer(creditorId);
            }

            return ContinueJourneyRedirect(nameof(CreditorAddAdHocDebt), new { debtId = model.DebtId, returnAction = model.ReturnAction });
        }

        [HttpGet]
        public IActionResult CreditorAddressSubmit(
            [FromQuery] Guid debtId = default,
            [FromQuery] string returnAction = null)
        {
            GenerateCountryListToViewBag();
            SetAdHocCreditorNameToViewBag();

            var model = new CreditorManualAddressSubmit();

            if (debtId != default)
            {
                CreditorResponse creditor = null;

                if (debtId != default && HttpContext.Session.Keys.Contains(_creditorKey))
                    creditor = JsonSerializer.Deserialize<CreditorResponse>(HttpContext.Session.GetString(_creditorKey));

                model = new CreditorManualAddressSubmit
                {
                    AddressId = creditor?.AddressId ?? default,
                    AddressLine1 = creditor?.AddressLine1,
                    AddressLine2 = creditor?.AddressLine2,
                    TownCity = creditor?.TownCity,
                    Postcode = creditor?.PostCode,
                    County = creditor?.County,
                    Country = creditor?.Country,
                    DebtId = debtId,
                    ReturnAction = returnAction
                };
            }
            else
            {
                var storedModel = GetJourneyObject<CreditorManualAddressSubmit>(nameof(CreditorPostcode));
                if (storedModel != null)
                {
                    model = storedModel;
                    TryValidateModel(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreditorAddressSubmit(CreditorManualAddressSubmit model)
        {
            SetJourneyObject(model, nameof(CreditorPostcode));

            if (!ModelState.IsValid)
            {
                GenerateCountryListToViewBag();
                SetAdHocCreditorNameToViewBag();

                return ContinueJourneyRedirect(nameof(CreditorAddressSubmit));
            }

            var creditorId = new Guid(HttpContext.Session.GetString(_creditorIdKey));
            model.AddressId = await _integrationGateway.AddAdHocCreditorAddressAsync(model, creditorId);

            SetJourneyObject(model, nameof(CreditorPostcode));

            bool.TryParse(HttpContext.Session.GetString(_isDebtTransferred), out var isDebtTransferred);
            if (isDebtTransferred)
            {
                return await SubmitSoldOnDebtAndRedirectToTransfer(creditorId);
            }

            TempData.Clear();
            return ContinueJourneyRedirect(nameof(CreditorAddAdHocDebt), new { debtId = model.DebtId, returnAction = model.ReturnAction });
        }

        #endregion

        #region Remove Debt

        [HttpGet]
        public async Task<IActionResult> DebtorRemoveDebt(Guid debtId)
        {
            var model = new DebtorRemoveDebtViewModel();

            var storedModel = GetJourneyObject<DebtorRemoveDebtViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model.DebtDetailViewModel = await GetDebtDetails(debtId);
            }

            TempData[_debtDetailViewModel] = JsonSerializer.Serialize(model.DebtDetailViewModel);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorRemoveDebt(DebtorRemoveDebtViewModel model)
        {
            var debtViewModel = JsonSerializer.Deserialize<DebtDetailViewModel>(TempData.Peek(_debtDetailViewModel).ToString());
            model.DebtDetailViewModel = debtViewModel;
            SetJourneyObject(model);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtorRemoveDebtConfirmation), new { debtId = debtViewModel.Debt.Id })
                : ContinueJourneyRedirect(nameof(DebtorRemoveDebt));
        }

        [HttpGet]
        public IActionResult DebtorRemoveDebtConfirmation()
        {
            var model = GetJourneyObject<DebtorRemoveDebtViewModel>(nameof(DebtorRemoveDebt));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorRemoveDebtConfirmation(DebtorRemoveDebtViewModel model)
        {
            var removeDebtRequest = new RemoveDebtRequest()
            {
                DebtId = model.DebtDetailViewModel.Debt.Id,
                MoneyAdviserNotes = model.MoreDetails,
                Reason = model.Reason.Value
            };
            var success = await _integrationGateway.RemoveDebt(removeDebtRequest);

            if (success)
            {
                _bannerService.ShowBanner(BannerTexts.DebtRemovedDirectlyBanner);
            }

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        #endregion

        #region Remove Presubmission Debt

        [HttpGet]
        public async Task<IActionResult> DebtorRemovePresubmissionDebt(Guid debtId)
        {
            var debtDetail = await GetDebtDetails(debtId);
            return View(debtDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorRemovePresubmissionDebtConfirmation([FromForm] Guid debtId)
        {
            var success = await _integrationGateway.RemoveDebtPresubmission(debtId);

            if (success)
            {
                _bannerService.ShowBanner(BannerTexts.DebtRemovedPresubmissionBanner);
            }

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        #endregion

        #region Transfer Debtor

        [HttpGet]
        public IActionResult DebtorTransfer()
        {
            var model = new DebtorTransferViewModel();

            var storedModel = GetJourneyObject<DebtorTransferViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model = new DebtorTransferViewModel(GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel));
            }

            TempData[_debtorTransferViewModel] = JsonSerializer.Serialize(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorTransfer(DebtorTransferViewModel model)
        {
            var debtorTransferViewModel = JsonSerializer.Deserialize<DebtorTransferViewModel>(TempData.Peek(_debtorTransferViewModel).ToString());
            debtorTransferViewModel.TransferReason = model.TransferReason;
            SetJourneyObject(debtorTransferViewModel);

            return ModelState.IsValid
                ? ContinueJourneyRedirect(nameof(DebtorTransferConfirmation))
                : ContinueJourneyRedirect(nameof(DebtorTransfer));
        }

        [HttpGet]
        public IActionResult DebtorTransferConfirmation()
        {
            var model = new DebtorTransferViewModel();

            var storedModel = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransferConfirmation));
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransfer));
            }

            SetJourneyObject(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorTransferConfirmation(DebtorTransferViewModel model)
        {
            var storedModel = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransferConfirmation));

            await _integrationGateway.TransferDebtor(new TransferDebtorRequest
            {
                TransferReason = storedModel.TransferReason,
                MoratoriumId = GetMoratoriumId()
            });

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public IActionResult DebtorTransferViewDetails() => View(GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel).DebtorTransfer);

        [HttpGet]
        public IActionResult DebtorTransferReview()
        {
            var model = new DebtorTransferViewModel();

            var storedModel = GetJourneyObject<DebtorTransferViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model = GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel).DebtorTransfer;
            }

            TempData[_debtorTransferViewModel] = JsonSerializer.Serialize(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DebtorTransferReview(DebtorTransferViewModel model)
        {
            var debtorTransferViewModel = JsonSerializer.Deserialize<DebtorTransferViewModel>(TempData.Peek(_debtorTransferViewModel).ToString());
            SetJourneyObject(debtorTransferViewModel);

            return ContinueJourneyRedirect(nameof(DebtorTransferReviewConfirmation));
        }

        [HttpGet]
        public IActionResult DebtorTransferReviewConfirmation()
        {
            var model = new DebtorTransferViewModel();

            var storedModel = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransferReviewConfirmation));
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                model = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransferReview));
            }

            SetJourneyObject(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorTransferReviewConfirmation(DebtorTransferViewModel model)
        {
            var storedModel = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransferReviewConfirmation));

            await _integrationGateway.CompleteTransferDebtor(GetMoratoriumId());

            return ContinueJourneyRedirect(nameof(DebtorTransferReviewConfirmationStatement));
        }

        [HttpGet]
        public IActionResult DebtorTransferReviewConfirmationStatement()
        {
            var storedModel = GetJourneyObject<DebtorTransferViewModel>(nameof(DebtorTransferReviewConfirmation));

            return View(storedModel);
        }

        [HttpGet]
        public IActionResult DebtorTransferAcknowledge()
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var model = new DebtorTransferAcknowledgeViewModel();

            var storedModel = GetJourneyObject<DebtorTransferAcknowledgeViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            else
            {
                var debtorTransferViewModel = GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel).DebtorTransfer;
                SetTempDataObject(_debtorTransferViewModel, debtorTransferViewModel);
                model.TransferDebtorDetails = debtorTransferViewModel;
                model.MoratoriumId = GetMoratoriumId();
            }

            SetJourneyObject(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebtorTransferAcknowledge(DebtorTransferAcknowledgeViewModel model)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            if (!ModelState.IsValid)
            {
                model.TransferDebtorDetails = GetTempDataObject<DebtorTransferViewModel>(_debtorTransferViewModel);
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(DebtorTransferAcknowledge));
            }

            if (model.CompleteTransfer.Value)
            {
                model.TransferDebtorDetails = GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel).DebtorTransfer;
                await _integrationGateway.AcknowledgeDebtorTransfer(model);
                _bannerService.ShowBanner(BannerTexts.DebtorTransferCompletedBanner);
            }

            return CompleteSubJourneyRedirect(nameof(DebtorAccountDetails));
        }

        [HttpGet]
        public IActionResult DebtorTransferCompleteDetails()
        {
            var debtorTransferViewModel = GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel).DebtorTransfer;

            return View(debtorTransferViewModel);
        }

        #endregion

        #region Private Methods

        private void ClearSession()
        {
            TempData.Clear();
            HttpContext.Session.Remove(_creditorIdKey);
            HttpContext.Session.Remove(_creditorKey);
            HttpContext.Session.Remove(_creditorNameKey);
            HttpContext.Session.Remove(_isDebtTransferred);
            HttpContext.Session.Remove(_debtorAddDebtFromCreateBreathingSpace);
            HttpContext.Session.Remove(_debtorAddNonCmpDebt);
            HttpContext.Session.Remove(_debtKey);
        }

        private IActionResult AddressSearch<T>(
            bool clear,
            string redirectFaiure,
            Guid addId = default,
            Guid debtId = default,
            Guid businessId = default,
            string returnAction = null,
            bool hasError = false
            ) where T : IAddressSearchViewModel, new()
        {
            var storedModel = GetJourneyObject<T>();
            T model;

            if (storedModel != null &&
                storedModel.AddressId != default &&
                addId == default)
            {
                addId = storedModel.AddressId;
            }

            if (storedModel is null || clear)
            {
                model = new T
                {
                    AddressId = addId,
                    DebtId = debtId,
                    BusinessId = businessId,
                    ReturnAction = returnAction,
                    OnPostRedirectAction = redirectFaiure,
                };
            }
            else
            {
                model = storedModel;
            }

            if (hasError)
            {
                TryValidateModel(model);
                var extraErrors = GetJourneyModelStateObject();
                if (extraErrors != null && extraErrors.Any())
                {
                    foreach (var error in extraErrors)
                        ModelState.AddModelError(error.Field, error.Error);
                }
            }

            return View(model);
        }

        private async Task<IActionResult> AddressSearch<T>(PostcodeSearchViewModel model)
            where T : IAddressSearchViewModel, new()
        {
            var extraErrors = new List<ModelFieldError>();
            var redirectErrorParams = new
            {
                addId = model.AddressId,
                returnAction = model.ReturnAction,
                hasError = true,
            };

            var mapModel = new T
            {
                Postcode = model.Postcode,
                AddressId = model.AddressId != null ? model.AddressId.Value : default,
                DebtId = model.DebtId != null ? model.DebtId.Value : default,
                BusinessId = model.BusinessId != null ? model.BusinessId.Value : default,
                ReturnAction = model.ReturnAction,
                OnPostRedirectAction = model.OnPostRedirectAction,
            };

            if (!ModelState.IsValid)
            {
                SetJourneyObject(mapModel);
                return ContinueJourneyRedirect(model.OnPostRedirectAction, redirectErrorParams);
            }

            var addressResult = await _addressLookupService.GetAddressesForPostcode(model.Postcode);

            if (!addressResult.IsValid)
            {
                foreach (var error in addressResult.Errors)
                {
                    ModelState.AddModelError(nameof(model.Postcode), error);
                    extraErrors.Add(new ModelFieldError(nameof(model.Postcode), error));
                }
            }

            if (!ModelState.IsValid)
            {
                SetJourneyObject(mapModel);
                SetJourneyModelStateObject(extraErrors);
                return ContinueJourneyRedirect(model.OnPostRedirectAction, redirectErrorParams);
            }

            mapModel.MapAddresses(addressResult);

            SetJourneyObject(mapModel);
            ClearJourneyModelStateObject();

            var redirectSuccessParams = new
            {
                addId = model.AddressId,
                returnAction = model.ReturnAction,
                hasError = false,
            };

            return ContinueJourneyRedirect(model.OnPostRedirectAction, redirectSuccessParams);
        }

        private async Task<IActionResult> DebtorAddressSaveAsync<T>(
            T model,
            string redirectFaiure,
            string redirectSuccess,
            Action<Address> additionalAddressMapping = null,
            Func<Address, Task<Guid>> saveAddress = null
        ) where T : IAddressSearchViewModel
        {
            var storedModel = GetJourneyObject<T>(redirectFaiure);
            model.Addresses = storedModel.Addresses;

            if (!ModelState.IsValid || !HasMoratoriumId())
            {
                SetJourneyObject(model, redirectFaiure);
                return ContinueJourneyRedirect(
                    redirectFaiure,
                    new
                    {
                        hasError = true,
                        addId = model.AddressId,
                        returnAction = model.ReturnAction,
                        debtId = model.DebtId,
                        businessId = model.BusinessId
                    });
            }

            var address = await _addressLookupService.GetFullAddressAsync(model.SelectedAddress);

            additionalAddressMapping?.Invoke(address);

            if (saveAddress != null)
                model.AddressId = await saveAddress(address);
            else
                model.AddressId = await _integrationGateway.CreateDebtorAddressAsync(address, GetMoratoriumId());

            SetJourneyObject(model, redirectFaiure);

            return CompleteSubJourneyRedirect(redirectSuccess);
        }

        private void GenerateCountryListToViewBag()
        {
            var countrySelect = new List<SelectListItem>(200) {
                new SelectListItem("Select a Country", Constants.UkCountryValue, true, true)
            };

            var mappedCountries = _countryList.Select(x => new SelectListItem(x, x));

            countrySelect.AddRange(mappedCountries);

            ViewBag.CountryList = countrySelect;
        }

        private void SetAdHocCreditorNameToViewBag()
        {
            var name = HttpContext.Session.GetString(_creditorNameKey);
            ViewBag.CreditorName = name;
        }

        private async Task<IActionResult> CreateAdHocCreditor(
            string name,
            Guid debtId = default,
            string returnAction = null,
            bool edit = false,
            bool isDebtTransferred = false)
        {
            var decodedName = HttpUtility.UrlDecode(name);
            var adHocCreditorId = await _integrationGateway.CreateAdHocCreditor(decodedName);

            HttpContext.Session.SetString(_creditorIdKey, adHocCreditorId.ToString());
            HttpContext.Session.SetString(_creditorNameKey, decodedName);
            HttpContext.Session.SetString(_isDebtTransferred, isDebtTransferred.ToString());
            SetTempDataObject(_debtKey, debtId);

            return ContinueJourneyRedirect(nameof(CreditorPostcode), new { debtId, returnAction, edit });
        }

        private async Task<IActionResult> SubmitSoldOnDebtAndRedirectToTransfer(
            Guid creditorId)
        {
            var debtId = GetTempDataObject<Guid>(_debtKey);
            var payload = new DebtSoldOnRequest { DebtId = debtId, SoldToCreditorId = creditorId };
            await _integrationGateway.SubmitDebtSold(payload);

            return ContinueJourneyRedirect(nameof(DebtorTransferDebt), new { debtId });
        }

        private async Task<DebtorAccountSummaryViewModel> GetAccountSummary()
        {
            var accountSummary = TempData[_bsSummaryViewModel] != null ?
                GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel) :
                await _integrationGateway.GetAccountSummary(GetMoratoriumId());

            accountSummary.IsOwningOrganization = accountSummary.MoneyAdviceOrganisation.Id.Equals(HttpContext.Session.GetOrganisation().Id);

            return accountSummary;
        }

        private async Task<DebtDetailViewModel> GetDebtDetails(Guid debtId)
        {
            if (TempData[_bsSummaryViewModel] != null)
            {
                var summaryModel = GetTempDataObject<DebtorAccountSummaryViewModel>(_bsSummaryViewModel);

                return summaryModel.DebtDetails
                    .FirstOrDefault(d => d?.Debt?.Id == debtId);
            }

            return await _integrationGateway.GetDebtDetail(debtId);
        }
        #endregion
    }
}
