using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Insolvency.Common;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models;
using Insolvency.Portal.Models.ViewModels;
using Insolvency.Portal.Models.ViewModels.Creditor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Insolvency.Portal.Models;

namespace Insolvency.Portal.Controllers
{
    [Authorize(Policy = Constants.Auth.CreditorPolicy)]
    public class CreditorController : BaseController
    {
        private const string _clientReviewModel = "ClientReviewModel";
        private const string _debtReviewModel = "DebtReviewModel";
        private const string _debts = "Debts";
        private const string _selectedDebt = "SelectedDebt";
        private const string _creditorSearchResults = "CreditorSearchResults";
        private const string _bannerHeader = "Confirmation";
        private const string _addressSearchModel = "AddressSearchModel";
        private const string _newCreditorId = "NewCreditorId";
        private const string _newCreditorName = "NewCreditorName";

        private readonly ILogger<CreditorController> _logger;
        private readonly ICreditorServiceGateway _creditorServiceGateway;
        private readonly IIntegrationGateway _integrationGateway;
        private readonly IAddressLookupGateway _addressLookupService;
        private readonly CountryList _countryList;

        public CreditorController(
            ICreditorServiceGateway creditorServiceGateway,
            IIntegrationGateway integrationGateway,
            IAddressLookupGateway addressLookupService,
            IHttpContextAccessor httpContextAccessor,
            CountryList countryList,
            ILogger<CreditorController> logger
       ) : base()
        {
            _creditorServiceGateway = creditorServiceGateway;
            _integrationGateway = integrationGateway;
            _addressLookupService = addressLookupService;
            _countryList = countryList;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction("Index", "BreathingSpace");

        [HttpGet]
        public async Task<IActionResult> BreathingSpace(CreditorBannerViewModel modelIn)
        {
            if (!HasMoratoriumId())
                return RedirectToHome();

            var moratoriumId = GetMoratoriumId();
            var moratorium = await _creditorServiceGateway.GetBreathingSpace(moratoriumId);

            if (moratorium == null)
                return RedirectToHome();

            var modelOut = new CreditorBreathingSpaceViewModel(moratorium);

            if (modelIn != null)
            {
                modelOut.BannerHeading = modelIn.BannerHeading;
                modelOut.BannerText = modelIn.BannerText;
            }

            TempData.Clear();
            SetTempDataObject(_debts, modelOut.Debts);

            return View(modelOut);
        }

        #region ClientEligibilityReview
        [HttpGet]
        public IActionResult ClientEligibilityReview()
        {
            var modelJson = TempData.Peek(_clientReviewModel)?.ToString();

            if (modelJson is null)
            {
                var storedModel = GetJourneyObject<CreditorClientEligibilityReviewViewModel>();
                if (storedModel != null)
                {
                    TryValidateModel(storedModel);
                    return View(storedModel);
                }

                return View();
            }

            var model = GetTempDataObject<CreditorClientEligibilityReviewViewModel>(_clientReviewModel);
            return View(model);
        }

        [HttpPost]
        public IActionResult ClientEligibilityReview(CreditorClientEligibilityReviewViewModel model)
        {
            SetJourneyObject(model);
            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(ClientEligibilityReview));
            }

            SetTempDataObject(_clientReviewModel, model);

            return ContinueJourneyRedirect(nameof(ClientEligibilityReviewConfirm));
        }

        [HttpGet]
        public IActionResult ClientEligibilityReviewConfirm()
        {
            var storedModel = GetJourneyObject<CreditorClientEligibilityReviewConfirmViewModel>();
            if (storedModel != null)
            {
                TryValidateModel(storedModel);
                return View(storedModel);
            }

            var model = GetTempDataObject<CreditorClientEligibilityReviewConfirmViewModel>(_clientReviewModel);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ClientEligibilityReviewConfirmPost(
            CreditorClientEligibilityReviewConfirmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetJourneyObject(model);
                return ContinueJourneyRedirect(nameof(ClientEligibilityReviewConfirm));
            }

            var moratoriumId = GetMoratoriumId();
            await _creditorServiceGateway.SubmitClientEligibilityReview(moratoriumId, model);
            SetJourneyObject(model);

            var modelOut = new CreditorBannerViewModel
            {
                BannerHeading = _bannerHeader,
                BannerText = "You requested a review of client eligibility",
            };
            return CompleteSubJourneyRedirect(nameof(BreathingSpace), modelOut);
        }
        #endregion

        #region DebtEligibilityReview
        [HttpGet]
        public IActionResult DebtEligibilityReview(int index)
        {
            var debts = GetTempDataObject<List<CreditorDebtPartialViewModel>>(_debts);

            var model = new CreditorDebtEligibilityReviewViewModel
            {
                Debt = debts[index]
            };

            var storedModel = GetJourneyObject<CreditorDebtEligibilityReviewViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }

            SetTempDataObject(_debtReviewModel, model);

            return View(model);
        }

        [HttpPost]
        public IActionResult DebtEligibilityReview(CreditorDebtEligibilityReviewViewModel model)
        {
            var debt = GetTempDataObject<CreditorDebtEligibilityReviewViewModel>(_debtReviewModel).Debt;
            model.Debt = debt;
            SetJourneyObject(model);

            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(DebtEligibilityReview));
            }

            SetTempDataObject(_debtReviewModel, model);

            return ContinueJourneyRedirect(nameof(DebtEligibilityReviewConfirm));
        }

        [HttpGet]
        public IActionResult DebtEligibilityReviewConfirm()
        {
            var storedModel = GetJourneyObject<CreditorDebtEligibilityReviewConfirmViewModel>();
            if (storedModel != null)
            {
                TryValidateModel(storedModel);
                return View(storedModel);
            }

            var model = GetTempDataObject<CreditorDebtEligibilityReviewConfirmViewModel>(_debtReviewModel);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DebtEligibilityReviewConfirmPost(
            CreditorDebtEligibilityReviewConfirmViewModel model)
        {
            SetJourneyObject(model, nameof(DebtEligibilityReviewConfirm));
            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(DebtEligibilityReviewConfirm));
            }

            await _creditorServiceGateway.SubmitDebtEligibilityReview(model);
            var modelOut = new CreditorBannerViewModel
            {
                BannerHeading = _bannerHeader,
                BannerText = "You’ve stopped all action against the debt and asked the money adviser for a review",
            };

            return CompleteSubJourneyRedirect(nameof(BreathingSpace), modelOut);
        }
        #endregion

        #region DebtStoppedAllAction
        [HttpGet]
        public IActionResult DebtStoppedAllAction(int index)
        {
            var storedModel = GetJourneyObject<CreditorDebtStoppedAllActionViewModel>();
            if (storedModel != null)
            {
                TryValidateModel(storedModel);
                return View(storedModel);
            }

            var debts = GetTempDataObject<List<CreditorDebtPartialViewModel>>(_debts);

            var model = new CreditorDebtStoppedAllActionViewModel
            {
                Debt = debts[index]
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DebtStoppedAllAction(CreditorDebtStoppedAllActionViewModel model)
        {
            SetJourneyObject(model);
            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(DebtStoppedAllAction));
            }

            await _creditorServiceGateway.SubmitDebtStoppedAllAction(model);
            var modelOut = new CreditorBannerViewModel
            {
                BannerHeading = _bannerHeader,
                BannerText = "You’ve stopped all action against the debt",
            };

            return CompleteSubJourneyRedirect(nameof(BreathingSpace), modelOut);
        }
        #endregion

        #region DebtSold
        [HttpGet]
        public IActionResult DebtSoldSearch(int index)
        {
            var debts = GetTempDataObject<List<CreditorDebtPartialViewModel>>(_debts);

            SetTempDataObject(_selectedDebt, debts[index]);

            var model = GetJourneyObject<CreditorSearchViewModel>();
            if (model != null)
            {
                TryValidateModel(model);
                return View(model);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DebtSoldSearch(CreditorSearchViewModel modelIn)
        {
            if (!ModelState.IsValid)
            {
                SetJourneyObject(modelIn);
                return ContinueJourneyRedirect(nameof(DebtSoldSearch));
            }

            var modelOut = await _integrationGateway.CmpCreditorSearch(HttpUtility.UrlDecode(modelIn.CreditorName));
            SetTempDataObject(_creditorSearchResults, modelOut.Creditors);

            if (modelOut.Creditors != null && modelOut.Creditors.Count > 1)
            {
                SetJourneyObject(modelOut);
                return ContinueJourneyRedirect(nameof(DebtSoldSearchResults), modelOut);
            }

            SetJourneyObject(modelIn);
            return await CreditorNewAdHocCreditor(modelIn.CreditorName, modelIn.DebtId);
        }

        [HttpGet]
        public IActionResult DebtSoldSearchResults()
        {
            var model = new CreditorSearchResultsViewModel();
            var storedModel = GetJourneyObject<CreditorSearchResultsViewModel>();
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult DebtSoldSearchResults(CreditorSearchResultsViewModel model)
        {
            model.Creditors = GetTempDataObject<List<SelectListItem>>(_creditorSearchResults);
            SetJourneyObject(model);

            if (!ModelState.IsValid)
            {
                return ContinueJourneyRedirect(nameof(DebtSoldSearchResults));
            }

            return ContinueJourneyRedirect(nameof(DebtSold), new { id = model.SelectedCreditor });
        }

        [HttpGet]
        public async Task<IActionResult> CreditorNewAdHocCreditor([FromQuery] string name, [FromQuery] Guid debtId)
        {
            var decodedName = HttpUtility.UrlDecode(name);
            var adHocCreditorId = await _integrationGateway.CreateAdHocCreditor(decodedName);

            HttpContext.Session.SetString(_newCreditorId, adHocCreditorId.ToString());
            HttpContext.Session.SetString(_newCreditorName, decodedName);
            SetJourneyObject(new CreditorSearchViewModel
            {
                CreditorName = name
            },
            nameof(DebtSoldSearch));

            return ContinueJourneyRedirect(nameof(CreditorPostcode), new { debtId, returnAction = nameof(DebtSold) });
        }

        [HttpGet]
        public IActionResult CreditorPostcode(
            [FromQuery] bool clear = false,
            [FromQuery] Guid debtId = default,
            [FromQuery] string returnAction = null,
            [FromQuery] bool hasError = false)
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

            if (!ModelState.IsValid || !HttpContext.Session.Keys.Contains(_newCreditorId))
            {
                SetJourneyObject(model);
                SetAdHocCreditorNameToViewBag();
                var storedModel = GetJourneyObject<AddressSearchViewModel>(nameof(CreditorPostcode));
                model.Addresses = storedModel.Addresses;
                return ContinueJourneyRedirect(nameof(CreditorPostcode), new { debtId = model.DebtId, hasError = true });
            }

            var creditorId = new Guid(HttpContext.Session.GetString(_newCreditorId));
            var address = await _addressLookupService.GetFullAddressAsync(model.SelectedAddress);
            await _integrationGateway.AddAdHocCreditorAddressAsync(address, creditorId);
            SetJourneyObject(model);

            return ContinueJourneyRedirect(nameof(DebtSold), new { id = creditorId });
        }

        [HttpGet]
        public IActionResult CreditorAddressSubmit()
        {
            GenerateCountryListToViewBag();
            SetAdHocCreditorNameToViewBag();

            var storedModel = GetJourneyObject<AddressWithValidation>(nameof(CreditorAddressSubmit));
            if (storedModel != null)
            {
                TryValidateModel(storedModel);
                return View(storedModel);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreditorAddressSubmit(AddressWithValidation creditorAddress)
        {
            SetJourneyObject(creditorAddress);
            if (!ModelState.IsValid)
            {
                GenerateCountryListToViewBag();
                SetAdHocCreditorNameToViewBag();
                return ContinueJourneyRedirect(nameof(CreditorAddressSubmit));
            }

            var creditorId = new Guid(HttpContext.Session.GetString(_newCreditorId));
            await _integrationGateway.AddAdHocCreditorAddressAsync(creditorAddress, creditorId);
            SetJourneyObject(creditorAddress);

            return ContinueJourneyRedirect(nameof(DebtSold), new { id = creditorId });
        }

        [HttpGet]
        public async Task<IActionResult> DebtSold([FromRoute] Guid id)
        {
            var creditor = await _integrationGateway.GetGenericCreditorByIdAsync(id.ToString());
            var debt = GetTempDataObject<CreditorDebtPartialViewModel>(_selectedDebt);


            var isAdHocCreditor = HttpContext.Session.TryGetValue(_newCreditorId, out _);

            var model = new CreditorDebtSoldViewModel
            {
                Creditor = creditor,
                Debt = debt,
                IsAdHocCreditor = isAdHocCreditor
            };

            var storedModel = GetJourneyObject<CreditorDebtSoldViewModel>(nameof(DebtSold));
            if (storedModel != null)
            {
                model = storedModel;
                TryValidateModel(model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DebtSold(CreditorDebtSoldViewModel model)
        {
            SetJourneyObject(model);

            await _creditorServiceGateway.SubmitDebtSold(model);
            var modelOut = new CreditorBannerViewModel
            {
                BannerHeading = _bannerHeader,
                BannerText = "We’ve told Advice UK you’ve sold the debt",
            };

            TempData.Remove(_addressSearchModel);
            HttpContext.Session.Remove(_newCreditorId);
            return CompleteSubJourneyRedirect(nameof(BreathingSpace), modelOut);
        }

        [HttpGet("creditor/ajax/creditor/search")]
        public async Task<IActionResult> CreditorSearchAjax([FromQuery] string query)
        {
            var searchResults = await _integrationGateway.CmpCreditorSearchAjax(HttpUtility.UrlDecode(query));

            return Json(searchResults);
        }
        #endregion

        #region Private Methods
        private void SetAdHocCreditorNameToViewBag()
        {
            var name = HttpContext.Session.GetString(_newCreditorName);
            ViewBag.CreditorName = name;
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

        #endregion
    }
}
