using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Interfaces;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models;
using Insolvency.Portal.Models.ViewModels;

namespace Insolvency.Portal.Gateways
{
    public class IntegrationGateway : IIntegrationGateway
    {
        public IApiClient Client { get; }

        public IntegrationGateway(IApiClient client) => Client = client;

        public async Task<AccountSearchViewModel> SearchAccounts(AccountSearchViewModel searchViewModel)
        {
            var parameters = new Dictionary<string, object>
            {
                { "BreathingSpaceReference", searchViewModel.Reference },
                { "Surname", searchViewModel.Surname }
            };
            if (searchViewModel.HasDateValue())
            {
                parameters.Add("DateOfBirth",
                    searchViewModel.GetBirthDate().ToString("o", CultureInfo.InvariantCulture));
            }
            var data = await Client.GetDataAsync<List<AccountSearchResponse>>("MoneyAdviser/BreathingSpaces/Search", parameters.ToArray());

            var resultViewModel = data?.Select(d => new SearchResultViewModel
            {
                Reference = d.BreathingSpaceReference,
                ClientName = new ClientName
                {
                    FirstName = d.FirstName,
                    MiddleName = d.MiddleName,
                    LastName = d.Surname,
                },
                DateOfBirth = d.DateOfBirth,
                Address = d.Address != null ? new Address(d.Address) : null,
                MoratoriumId = d.BreathingSpaceId,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                MoratoriumStatus = d.MoratoriumStatus,
                OrganisationName = d.OrganisationName,
                MoratoriumType = d.MoratoriumType
            }) ?? Enumerable.Empty<SearchResultViewModel>();

            if (searchViewModel.PageNumber > 1)
            {
                resultViewModel = resultViewModel
                    .Skip(searchViewModel.PageSize * (searchViewModel.PageNumber - 1))
                    .Take(searchViewModel.PageSize);
            }
            else
            {
                resultViewModel = resultViewModel.Take(searchViewModel.PageSize);
            }

            searchViewModel.SearchResultViewModel = new AccountSearchResultViewModel
            {
                SearchResultView = resultViewModel
                    .OrderByDescending(s => s.CreatedOn)
                    .OrderByDescending(s => s.MoratoriumStatus == MoratoriumStatus.Active)
                    .ThenByDescending(s => s.MoratoriumStatus == MoratoriumStatus.Draft)
                    .ThenByDescending(s => s.MoratoriumStatus == MoratoriumStatus.Expired)
                    .ToList(),
                SearchParameterView = new SearchParameterViewModel
                (
                    searchViewModel.Reference,
                    searchViewModel.Surname,
                    searchViewModel.HasDateValue()
                        ? searchViewModel.GetBirthDate().ToString(Constants.UkDateFormat)
                        : string.Empty
                ),
                SearchPagination = new Pagination(
                    data?.Count() ?? 0,
                    nameof(searchViewModel.PageNumber),
                    resultViewModel.Count(),
                    searchViewModel.PageNumber
                )
            };

            return searchViewModel;
        }

        public async Task<Guid> CreateClientAsync(ClientDetailsCreateViewModel model)
        {
            var mappedModel = new ClientDetailsCreateRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                DateOfBirth = new DateTime(model.BirthYear.Value, model.BirthMonth.Value, model.BirthDay.Value)
            };

            return await Client.CreateAsync<Guid, ClientDetailsCreateRequest>(mappedModel, "api/debtor");
        }

        public async Task<Guid> AddClientPreviousNameAsync(ClientAddPreviousNameViewModel model, Guid moratoriumId)
        {
            var mappedModel = new ClientPreviousNameCreateRequest
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                MoratoriumId = moratoriumId
            };

            return await Client.CreateAsync<Guid, ClientPreviousNameCreateRequest>(mappedModel, "api/debtor/previousname/add");
        }

        public async Task<Guid> UpdateClientPreviousNameAsync(ClientAddPreviousNameViewModel model, Guid moratoriumId)
        {
            var mappedModel = new ClientPreviousNameUpdateRequest
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                MoratoriumId = moratoriumId,
                NameId = model.PreviousNameId
            };

            return await Client.CreateAsync<Guid, ClientPreviousNameUpdateRequest>(mappedModel, "api/debtor/previousname/update");
        }

        public async Task<ClientNamesSummaryViewModel> GetClientNamesAsync(Guid moratoriumId)
        {
            var queryParams = new Dictionary<string, object> { { nameof(moratoriumId), moratoriumId } };

            var debtorDetails = await Client.GetDataAsync<DebtorDetailsResponse>("/api/debtor/names/all", queryParams.ToArray());

            return new ClientNamesSummaryViewModel(debtorDetails);
        }

        public async Task UpdateClientAsync(ClientDetailsCreateViewModel model)
        {
            var mappedModel = new ClientDetailsUpdateRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                DateOfBirth = new DateTime(model.BirthYear.Value, model.BirthMonth.Value, model.BirthDay.Value)
            };

            await Client.UpdateAsync<ClientDetailsUpdateRequest>(mappedModel, $"MoneyAdviser/BreathingSpaces/{model.MoratoriumId}/Debtor");
        }

        public async Task<DebtorAccountConfirmationViewModel> SubmitDebtorAccount(DebtorRadioYesNoViewModel model)
        {
            var mappedModel = new DebtorAccountSaveRequest
            {
                MoratoriumId = model.MoratoriumId
            };

            var confirmationDetails = await Client.CreateAsync<DebtorAccountSaveResponse, DebtorAccountSaveRequest>(mappedModel, "api/debtor/save");

            return new DebtorAccountConfirmationViewModel
            {
                MoratoriumReference = confirmationDetails.MoratoriumReference
            };
        }

        public async Task<Guid> AddAdHocCreditorAddressAsync(Address creditorAddress, Guid creditorId)
        {
            var integrationModel = new CustomAddress
            {
                AddressLine1 = creditorAddress.AddressLine1,
                AddressLine2 = creditorAddress.AddressLine2,
                County = creditorAddress.County,
                Postcode = creditorAddress.Postcode,
                TownCity = creditorAddress.TownCity,
                Country = creditorAddress.Country ?? Constants.UkCountryValue,
                OwnerId = creditorId
            };

            var result = await Client.CreateAsync<Guid, CustomAddress>(integrationModel, "OnboardedCreditors/customaddress");

            return result;
        }

        public async Task<Guid> CreateDebtorAddressAsync(Address debtorAddress, Guid moratoriumId)
        {
            if (debtorAddress.AddressId != default)
            {
                await UpdateAddressAsync(debtorAddress, moratoriumId);
                return debtorAddress.AddressId;
            }

            return await CreateAddressAsync(debtorAddress, moratoriumId);
        }

        public async Task<Guid> CreateDebtorPreviousAddressAsync(Address debtorAddress, Guid moratoriumId)
        {
            if (debtorAddress.AddressId != default)
            {
                await UpdatePreviousAddressAsync(debtorAddress, moratoriumId);
                return debtorAddress.AddressId;
            }

            return await CreateAddressAsync(debtorAddress, moratoriumId);
        }

        public async Task<Guid> CreateAddressAsync(Address debtorAddress, Guid moratoriumId)
        {
            var integrationModel = new CustomAddress
            {
                AddressLine1 = debtorAddress.AddressLine1,
                AddressLine2 = debtorAddress.AddressLine2,
                County = debtorAddress.County,
                Postcode = debtorAddress.Postcode,
                TownCity = debtorAddress.TownCity,
                Country = debtorAddress.Country ?? Constants.UkCountryValue,
                OwnerId = moratoriumId,
                DateFrom = debtorAddress.DateFrom,
                DateTo = debtorAddress.DateTo
            };

            var result = await Client.CreateAsync<Guid, CustomAddress>(integrationModel, "api/debtor/customaddress");

            return result;
        }

        public async Task UpdateAddressAsync(Address debtorAddress, Guid moratoriumId) => await Client.UpdateAsync<CustomAddress>(debtorAddress.ToCustomAddress(), $"MoneyAdviser/BreathingSpaces/{moratoriumId}/Addresses/{debtorAddress.AddressId}");

        public async Task UpdatePreviousAddressAsync(Address debtorAddress, Guid moratoriumId) => await Client.UpdateAsync<UpdateCustomAddress>(debtorAddress.ToUpdateCustomAddress(moratoriumId), "api/debtor/previous-address");

        public async Task UpdateBusinessNameAsync(DebtorAddBusinessViewModel businessViewModel)
        {
            var businessNameUpdateRequest = new BusinessNameUpdateRequest
            {
                BusinessName = businessViewModel.BusinessName
            };

            await Client.UpdateAsync<BusinessNameUpdateRequest>(businessNameUpdateRequest, $"MoneyAdviser/BreathingSpaces/{businessViewModel.MoratoriumId}/Businesses/{businessViewModel.BusinessId}");
        }

        public async Task<Guid> CreatDebtAsync(DebtViewModel model)
        {
            decimal? amount = null;
            var flag = decimal.TryParse(model.DebtAmount, out var parsedAmount);
            if (flag) amount = parsedAmount;

            var debt = new CreateDebtRequest
            {
                DebtTypeId = model.SelectedDebtTypeId,
                // if there is no debt id, it means we are adding an ad hoc debt
                // and if there is we don't want to polute the OtherDebtType field on Dynamics
                DebtTypeName = model.SelectedDebtTypeId is null ? model.SelectedDebtTypeName : null,
                NINO = model.NINO,
                Reference = model.Reference,
                Amount = amount,
                CreditorId = model.CreditorId
            };

            var result = await Client.CreateAsync<CreateDebtResponse, CreateDebtRequest>(debt, $"MoneyAdviser/BreathingSpaces/{model.MoratoriumId}/Debt");

            return result.DebtId;
        }

        public async Task<Guid> UpdateDebtAsync(DebtViewModel model)
        {
            decimal? amount = null;
            var flag = decimal.TryParse(model.DebtAmount, out var parsedAmount);
            if (flag) amount = parsedAmount;

            var debt = new UpdateDebtRequest
            {
                DebtId = model.Id,
                DebtTypeId = model.SelectedDebtTypeId,
                // if there is no debt id, it means we are adding an ad hoc debt
                // and if there is we don't want to polute the OtherDebtType field on Dynamics
                DebtTypeName = model.SelectedDebtTypeId is null ? model.SelectedDebtTypeName : null,
                NINO = model.NINO,
                Reference = model.Reference,
                Amount = amount,
                CreditorId = model.CreditorId
            };

            var result = await Client.CreateAsync<CreateDebtResponse, UpdateDebtRequest>(debt, $"MoneyAdviser/BreathingSpaces/Debt/Update");

            return result.DebtId;
        }

        public async Task<CreditorResponse> GetGenericCreditorByIdAsync(string id)
        {
            var creditor = await Client.GetDataAsync<CreditorResponse>($"OnboardedCreditors/{id}");
            return creditor;
        }

        public async Task<CreditorSearchResultsViewModel> CmpCreditorSearch(string creditorName)
        {
            var queryParams = new Dictionary<string, object> { { "creditorName", creditorName } };
            var creditorSearchResults = await Client.GetDataAsync<CreditorSearchResponse>("OnboardedCreditors/search", queryParams.ToArray());

            var modelOut = new CreditorSearchResultsViewModel();
            modelOut.MapCreditors(creditorSearchResults);

            return modelOut;
        }

        public async Task<CreditorSearchResponse> CmpCreditorSearchAjax(string creditorName)
        {
            var queryParams = new Dictionary<string, object> { { "creditorName", creditorName } };
            var creditorSearchResults = await Client.GetDataAsync<CreditorSearchResponse>("OnboardedCreditors/search", queryParams.ToArray());

            return creditorSearchResults;
        }

        public async Task<DebtorDetailViewModel> GetDebtorConfirmDetails(Guid moratoriumId)
        {
            var queryParams = new Dictionary<string, object> { { nameof(moratoriumId), moratoriumId } };

            var accountSummary = await Client.GetDataAsync<BreathingSpaceResponse>("api/debtor/details", queryParams.ToArray());

            return new DebtorDetailViewModel(accountSummary);
        }

        public async Task SetBreathingSpaceAsMentalHealth(Guid moratoriumId) =>
            await Client.CreateAsync<string>($"api/debtor/{moratoriumId}/set-as-mental-health");

        public async Task SetBreathingSpaceAsStandard(Guid moratoriumId) =>
            await Client.CreateAsync<string>($"api/debtor/{moratoriumId}/set-as-standard");

        public async Task<DebtorAccountSummaryViewModel> GetAccountSummary(Guid moratoriumId)
        {
            var debtorAccountSummary = await Client.GetDataAsync<BreathingSpaceResponse>($"MoneyAdviser/BreathingSpaces/{moratoriumId}");

            return new DebtorAccountSummaryViewModel(debtorAccountSummary);
        }

        public async Task BrowseBreathingSpacesAsync(BreathingSpaceBrowseViewModel model)
        {
            var request = new BreathingSpaceBrowseRequest()
            {
                Category = model.BrowseByCategory,
                Task = model.BrowseByTask,
                ShowNewestFirst = model.ShowNewestFirst,
                Page = model.Page
            };

            var results = await Client.CreateAsync<BreathingSpaceBrowseResponse, BreathingSpaceBrowseRequest>(request, "MoneyAdviser/BreathingSpaces/Browse");
            model.BreathingSpaceBrowseItems = results.BreathingSpaceBrowseItems;

            model.Pagination = new Pagination(
                model.GetTotalItemsCount(),
                nameof(model.Page),
                results.BreathingSpaceBrowseItems.Count,
                model.Page ?? 1,
                Constants.PageSize);
        }

        public async Task<MoneyAdviserLandingPageViewModel> GetMoneyAdviserLandingPageStats()
        {
            var landingPageStats = await Client.GetDataAsync<MoneyAdviserLandingPageStatsResponse>($"MoneyAdviser/LandingPageStats");
            return new MoneyAdviserLandingPageViewModel(landingPageStats);
        }

        public async Task<DebtorAddressViewModel> GetDebtorAddresses(Guid moratoriumId)
        {
            var queryParams = new Dictionary<string, object> { { nameof(moratoriumId), moratoriumId } };

            var debtorAccountSummary = await Client.GetDataAsync<BreathingSpaceResponse>("api/debtor/details", queryParams.ToArray());

            return new DebtorAddressViewModel(debtorAccountSummary);
        }

        public async Task<Guid> DebtorHideAddressAsync(DebtorRadioYesNoViewModel model)
        {
            var integrationModel = new DebtorHideAddressRequest
            {
                HideAddress = model.SubmitNow
            };

            return await Client.UpdateAsync<Guid, DebtorHideAddressRequest>(integrationModel, $"MoneyAdviser/BreathingSpaces/{model.MoratoriumId}/Addresses/Hidden");
        }

        public async Task<BusinessAdressResponse> DebtorAddBusinessAsync(DebtorAddBusinessViewModel businessAddress)
        {
            var integrationModel = businessAddress.ToCreateBusinessModel();
            var result = await Client.CreateAsync<BusinessAdressResponse, BusinessAddressRequest>(integrationModel, "api/debtor/add-business-address");
            return result;
        }

        public async Task DebtorUpdateBusinessAsync(DebtorAddBusinessViewModel businessAddress)
        {
            await UpdateAddressAsync(businessAddress.DebtorCurrentAddress, businessAddress.MoratoriumId);
            await UpdateBusinessNameAsync(businessAddress);
        }

        public async Task<NominatedContactCreateResponse> CreateNominatedContactAsync(DebtorNominatedContactViewModel viewModel)
        {
            var integrationModel = viewModel.ToCreateNominatedContactRequestModel();
            var result = await Client.CreateAsync<NominatedContactCreateResponse, NominatedContactCreateRequest>(integrationModel, "api/debtor/add-nominated-contact");
            return result;
        }

        public async Task UpdateNominatedContactAsync(DebtorNominatedContactViewModel viewModel)
        {
            var integrationModel = viewModel.ToUpdateNominatedContactRequestModel();
            await Client.UpdateAsync(integrationModel, $"MoneyAdviser/BreathingSpaces/{viewModel.MoratoriumId}/NominatedContacts/{viewModel.ContactId}");
        }

        public async Task<Guid> CreateAdHocCreditor(string name)
        {
            var model = new AdHocCreditorRequst { Name = name };
            return await Client.CreateAsync<Guid, AdHocCreditorRequst>(model, "OnboardedCreditors/adhoc");
        }

        public async Task DebtorSetContactPreference(Guid moratoriumId, DebtorContactPreferenceViewModel model)
        {
            var payload = new DebtorContactPreferenceRequest
            {
                EmailAddress = model.EmailAddress,
                Preference = model.SubmitOption.Value
            };

            await Client.UpdateAsync<DebtorContactPreferenceRequest>(payload, $"MoneyAdviser/BreathingSpaces/{moratoriumId}/Debtor/ContactPreferences");
        }

        public async Task<DebtorNominatedContactSummaryViewModel> GetDebtorNominatedContactSummary(Guid moratoriumId)
        {
            var queryParams = new Dictionary<string, object> { { nameof(moratoriumId), moratoriumId } };
            var nominatedContact =
                await Client.GetDataAsync<DebtorNominatedContactResponse>("api/debtor/nominated-contact-summary", queryParams.ToArray());
            return new DebtorNominatedContactSummaryViewModel(nominatedContact);
        }

        public async Task DebtorEndAccount(Guid moratoriumId, DebtorAccountEndReasonConfirmationViewModel model)
        {
            var payload = new DebtorAccountEndRequest
            {
                Reason = model.SubmitOption,
                IsPartOfThirtyDayReview = model.IsPartOfThirtyDayReview ?? null,
                NoLongerEligibleReason = model.NoLongerEligibleReason ?? null,
                EndTreatmentDate = model.EndTreatmentDate,
                DateOfDeath = model.DateOfDeath
            };
            await Client.CreateAsync(payload, $"MoneyAdviser/BreathingSpaces/{moratoriumId}/End");
        }

        public async Task<DebtDetailViewModel> GetDebtDetail(Guid debtId)
        {
            var debtorDetails = await Client.GetDataAsync<DebtDetailResponse>($"api/debtor/debt/{debtId}");

            return new DebtDetailViewModel(debtorDetails, null);
        }

        public async Task<Guid> SubmitDebtEligibilityReview(DebtEligibilityReviewSummaryViewModel viewModel)
        {
            var reviewModel = new DebtEligibilityReviewResponseRequest
            {
                DebtId = viewModel.DebtDetailViewModel.Debt.Id,
                MoneyAdviserNotes = viewModel.ReviewSupportingDetail,
                RemoveAfterReview = viewModel.SubmitNow
            };

            var success = await Client.CreateAsync(reviewModel, $"MoneyAdviser/Debts/{reviewModel.DebtId}/EligibilityReviews/MakeDetermination");

            if (success)
                return reviewModel.DebtId;

            return Guid.Empty;
        }

        public async Task<Guid> DebtorReviewClientEligibility(Guid moratoriumId, DebtorEligibilityReviewDecisionViewModel model)
        {
            var payload = new DebtorEligibilityReviewResponseRequest
            {
                MoneyAdviserNotes = model.MoneyAdviserNotes,
                IsNotEligibleAfterReview = model.EndBreathingSpace.Value,
                CreditorId = model.CreditorId,
            };

            return await Client.CreateAsync<Guid, DebtorEligibilityReviewResponseRequest>(payload, $"MoneyAdviser/BreathingSpaces/{moratoriumId}/EligibilityReviews/MakeDetermination");
        }

        public async Task<Guid> SubmitDebtorEligibilityReviewTask(DebtorEligibilityReviewSummaryViewModel model)
        {
            var payload = new MaDebtorEligibilityReviewRequest
            {
                CreditorId = model.CreditorId,
                CreditorNotes = model.CreditorNotes,
                EndReason = model.EndResaon.Value,
                NoLongerEligibleReason = model.NoLongerEligibleReason,
                MoratoriumId = model.MoratoriumId
            };

            return await Client.CreateAsync<Guid, MaDebtorEligibilityReviewRequest>(payload, $"MoneyAdviser/BreathingSpaces/{payload.MoratoriumId}/EligibilityReviews");
        }

        public async Task<Guid> SubmitDebtEligibilityReviewTask(DebtElgibilityReviewTaskSummaryViewModel model)
        {
            var payload = new DebtEligibilityReviewRequest
            {
                CreditorNotes = model.CreditorNotes,
                ReviewType = model.Reason.Value,
                DebtId = model.DebtDetailViewModel.Debt.Id
            };

            return await Client.CreateAsync<Guid, DebtEligibilityReviewRequest>(payload, $"MoneyAdviser/Debts/{payload.DebtId}/EligibilityReviews");
        }

        public async Task SubmitDebtSold(DebtSoldOnRequest model)
        {
            await Client.CreateAsync<object, DebtSoldOnRequest>(model, $"MoneyAdviser/Debts/{model.DebtId}/DebtHasBeenSold");
            return;
        }

        public async Task ConfirmDebtSold(Guid debtId) => await Client.CreateAsync<string>($"MoneyAdviser/Debts/{debtId}/AcceptSoldNotification");

        public async Task<bool> RemoveDebt(RemoveDebtRequest removeDebtRequest) => await Client.CreateAsync<object>(removeDebtRequest, $"MoneyAdviser/Debts/{removeDebtRequest.DebtId}/End");
        public async Task<bool> RemoveDebtPresubmission(Guid debtId) => await Client.DeleteAsync($"api/debtor/debt/{debtId}");
        public async Task MakeProposedDebtDetermination(ReviewProposedDebtRequest reviewProposedDebtRequest) => await Client.CreateAsync(reviewProposedDebtRequest, $"MoneyAdviser/Debts/{reviewProposedDebtRequest.DebtId}/MakeProposedDebtDetermination");
        public async Task TransferDebtor(TransferDebtorRequest transferDebtorRequest) => await Client.CreateAsync(transferDebtorRequest, $"MoneyAdviser/BreathingSpaces/{transferDebtorRequest.MoratoriumId}/Transfer");
        public async Task CompleteTransferDebtor(Guid moratoriumIdId) => await Client.CreateAsync<object>($"MoneyAdviser/BreathingSpaces/{moratoriumIdId}/CompleteTransfer");
        public async Task AcknowledgeDebtorTransfer(DebtorTransferAcknowledgeViewModel model)
        {
            var payload = new DebtorTransferRequest
            {
                AcknowledgingOrganisationId = model.TransferDebtorDetails.MoneyAdviceOrganisation.Id,
                AcknowledgingMoneyAdviserName = model.TransferDebtorDetails.RequestingMoneyAdviceOrganisation
            };
            await Client.CreateAsync(payload, $"api/debtor/{model.MoratoriumId}/transfer/acknowledge");
        }
    }
}
