using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Common.Enums;
using Insolvency.Common.Exceptions;
using Insolvency.Integration.Gateways.Audit;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Gateways.MoratoriumEntities;
using Insolvency.Integration.Gateways.OData;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.BreathingSpaceCreation;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Microsoft.Extensions.Logging;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways
{
    public class MoneyAdviserServiceDynamicsGateway : IMoneyAdviserServiceDynamicsGateway
    {
        private readonly ILogger<MoneyAdviserServiceDynamicsGateway> _logger;
        private readonly IODataClient _client;
        private readonly DynamicsGatewayOptions _options;
        private readonly AuditContext _auditContext;
        private readonly IAuditService _auditService;

        public MoneyAdviserServiceDynamicsGateway(
            IODataClient client,
            DynamicsGatewayOptions options,
            ILogger<MoneyAdviserServiceDynamicsGateway> logger,
            AuditContext auditContext,
            IAuditService auditService)
        {
            _logger = logger;
            _client = client;
            _options = options;
            _auditContext = auditContext;
            _auditService = auditService;
        }

        public async Task<MoneyAdviserLandingPageStatsResponse> GetMoneyAdviserLandingPageStats(Guid organisationId)
        {
            var homePageStatsResponse = await _client.For<inss_moneyadviserorganisation>()
                 .Key(organisationId)
                 .Action("ntt_BSSMoneyAdviserOrganisationGetHomepage")
                 .ExecuteAsSingleAsync<string>();
            var homePageStats = JsonSerializer.Deserialize<LandingPageStats>(homePageStatsResponse);

            return new MoneyAdviserLandingPageStatsResponse
            {
                ActiveBreathingSpacesCount = homePageStats.ActiveMoratoriums,
                ClientTransferRequests = homePageStats.ClientTransferRequests,
                ClientTransfers = homePageStats.ClientTransfers,
                DebtEligibilityReviews = homePageStats.DebtEligibilityReviews,
                DebtorEligibilityReviews = homePageStats.DebtorEligibilityReviews,
                DebtProposedDebts = homePageStats.DebtProposedDebts,
                SoldOnDebts = homePageStats.SoldOnDebts,
                SentToMoneyAdviser = homePageStats.SentToMoneyAdviser
            };
        }

        public async Task<Guid> CreateClientWithDetailsAsync(ClientDetailsCreateRequest model, Guid organisationId)
        {
            var dynamicsDebtorModel = new Ntt_breathingspacedebtor
            {
                ntt_firstname = model.FirstName,
                ntt_lastname = model.LastName,
                ntt_middlename = model.MiddleName,
                ntt_dateofbirth = model.DateOfBirth
            };

            var debtorResult = await _client.For<Ntt_breathingspacedebtor>()
                .Set(dynamicsDebtorModel)
                .InsertEntryWithLogsAsync(_logger);

            var moneyAdviser = await _client.For<inss_moneyadviserorganisation>()
                .Key(organisationId)
                .FindEntryAsync();

            var moratorium = new Ntt_breathingspacemoratorium
            {
                ntt_ManagingMoneyAdviserOrganisationId = moneyAdviser
            };

            var moratoriumResult = await _client.For<Ntt_breathingspacemoratorium>()
                .Set(moratorium)
                .InsertEntryWithLogsAsync(_logger);

            await _client.For<Ntt_breathingspacedebtor>()
                .Key(debtorResult.Ntt_breathingspacedebtorid.Value)
                .LinkEntryWithLogsAsync(_logger, moratoriumResult, "Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid");

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(dynamicsDebtorModel.ToAuditDictionary()));

            return moratoriumResult.ntt_breathingspacemoratoriumid.Value;
        }

        public async Task UpdateClientWithDetailsAsync(ClientDetailsUpdateRequest model)
        {
            var breathingSpace = await _client.For<Ntt_breathingspacemoratorium>()
                                        .Key(model.MoratoriumId)
                                        .Select(m => new
                                        {
                                            m._ntt_debtorid_value,
                                            m._ntt_breathingspacestatusid_value
                                        })
                                        .FindEntryAsync();
            if (breathingSpace == null)
            {
                throw new NotFoundHttpResponseException();
            }

            var bsStatus = breathingSpace.GetMoratoriumStatus();
            if (bsStatus != MoratoriumStatus.Active && bsStatus != MoratoriumStatus.Draft)
            {
                throw new ConflictHttpResponseException();
            }

            var result = await _client.For<Ntt_breathingspacedebtor>()
                .Key(breathingSpace._ntt_debtorid_value)
                .Set(new
                {
                    ntt_firstname = model.FirstName,
                    ntt_lastname = model.LastName,
                    ntt_middlename = model.MiddleName ?? string.Empty,
                    ntt_dateofbirth = model.DateOfBirth
                })
                .UpdateEntryWithLogsAsync(_logger);

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(result.ToAuditDictionary()));
        }

        public async Task<Guid> AddClientPreviousNameAsync(ClientPreviousNameCreateRequest model)
        {
            var dynamicsPreviousNameModel = new ntt_previousname
            {
                ntt_firstname = model.FirstName,
                ntt_middlename = model.MiddleName,
                ntt_lastname = model.LastName,
            };

            var previousNameResult = await _client.For<ntt_previousname>()
            .Set(dynamicsPreviousNameModel)
            .InsertEntryWithLogsAsync(_logger);


            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Expand(x => x.ntt_debtorid)
                .FindEntryAsync();

            await _client.For<Ntt_breathingspacedebtor>()
                  .Key(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid.Value)
                  .LinkEntryWithLogsAsync(_logger, previousNameResult, "ntt_breathingspacedebtor_ntt_previousname_DebtorId");

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(previousNameResult.ToAuditDictionary()));

            return previousNameResult.GetId();
        }

        public async Task<Guid> UpdateClientPreviousNameAsync(ClientPreviousNameUpdateRequest model)
        {
            var result = await _client
                .For<ntt_previousname>()
                .Key(model.NameId)
                .Set(new
                {
                    ntt_firstname = model.FirstName,
                    ntt_middlename = model.MiddleName ?? string.Empty,
                    ntt_lastname = model.LastName,
                })
                .UpdateEntryWithLogsAsync(_logger);

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(result.ToAuditDictionary()));

            return result.GetId();
        }

        public async Task<Guid> CreateDebtorAddressAsync(CustomAddress model)
        {
            var dynamicsModel = new Inss_InssAddress
            {
                inss_addressline1 = model.AddressLine1,
                inss_addressline2 = model.AddressLine2,
                inss_postcode = model.Postcode,
                inss_addressline4 = model.County,
                inss_addressline3 = model.TownCity,
                inss_country = model.Country,
                inss_datefrom = model.DateFrom,
                inss_dateto = model.DateTo
            };

            var result = await _client.For<Inss_InssAddress>()
                 .Set(dynamicsModel)
                 .InsertEntryWithLogsAsync(_logger);

            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.OwnerId)
                .Expand(x => x.ntt_debtorid)
                .FindEntryAsync();

            await _client.For<Ntt_breathingspacedebtor>()
                .Key(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid)
                .LinkEntryWithLogsAsync(_logger, result, "ntt_breathingspacedebtor_inss_inssaddress");

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(result.ToAuditDictionary()));

            return result.inss_inssaddressid.Value;
        }

        public async Task UpdateAddressAsync(UpdateCustomAddress model)
        {
            var postcodeClean = (model.Postcode is null && model.Country != Constants.UkCountryValue)
                ? string.Empty
                : model.Postcode;

            var actionParameters = new Dictionary<string, object>()
               {
                   { "AddressId", model.AddressId },
                   { "AddressLine1", model.AddressLine1 },
                   { "AddressLine2", model.AddressLine2 },
                   { "AddressLine3", model.TownCity },
                   { "AddressLine4", model.County },
                   { "Country", model.Country },
                   { "Postcode", postcodeClean },
                   { "DateFrom", model.DateFrom.HasValue ? model.DateFrom.Value.ToString(Constants.UkDateFormat) : null },
                   { "DateTo", model.DateTo.HasValue ? model.DateTo.Value.ToString(Constants.UkDateFormat) : null }
               };

            await _client
               .For<Ntt_breathingspacemoratorium>()
               .Key(model.MoratoriumId)
               .Action("ntt_BSSAPIMAAddressUpdate")
               .Set(new Dictionary<string, object>(actionParameters).SetDynamicsActionAuditParameters(_auditContext))
               .ExecuteAsync();

            actionParameters.Add("ntt_BSSAPIMAAddressUpdate", true);

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(actionParameters));
        }

        public async Task<DebtorAccountSaveResponse> SaveDebtorAsync(DebtorAccountSaveRequest model)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Expand(m => m.ntt_breathingspacemoratorium_ntt_breathingspacedebt_BreathingSpaceMoratoriumId)
                .FindEntryAsync();

            var moratoriumId = moratorium.ntt_breathingspacemoratoriumid;

            var hasDebts = moratorium.ntt_breathingspacemoratorium_ntt_breathingspacedebt_BreathingSpaceMoratoriumId.Any();
            if (!hasDebts)
            {
                throw new UnprocessableEntityHttpResponseException("There must be one (1) eligible debt per breathing space");
            }

            var moratoriumReference = await _client.For<Ntt_breathingspacemoratorium>()
                  .Key(moratoriumId)
                  .Action("ntt_bssmoratoriumactivate")
                  .Set(_auditContext.GenerateActionParameters())
                  .ExecuteAsSingleAsync<string>();

            var auditDetail = _auditContext.ToAuditDetail(
                new Dictionary<string, object>
                {
                    { nameof(model.MoratoriumId), model.MoratoriumId },
                    { "ntt_bssmoratoriumactivate", true }
                });
            await _auditService.PerformAuditing(auditDetail);

            return new DebtorAccountSaveResponse
            {
                MoratoriumReference = moratoriumReference
            };
        }

        public async Task<Guid> CreateDebtAsync(CreateDebtRequest model)
        {
            var parameters = model
                .ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            var result = await _client.ExecuteActionAsSingleAsync("ntt_BSSDebtCreate", parameters);
            var debtId = result["DebtId"].ToString();

            var contentDict = model.ToDictionary();
            parameters.Add("ntt_BSSDebtCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return new Guid(debtId);
        }

        public async Task<Guid> UpdateDebtAsync(UpdateDebtRequest model)
        {
            await _client.For<Ntt_breathingspacedebt>()
                  .Key(model.DebtId)
                  .Action("ntt_BSSDebtUpdate")
                  .Set(model.ToDictionary().SetDynamicsActionAuditParameters(_auditContext))
                  .ExecuteAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.DebtId), model.DebtId);
            contentDict.Add("ntt_BSSDebtUpdate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return model.DebtId;
        }

        public async Task<Guid> SetBreathingSpaceAsMentalHealth(Guid moratoriumId)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(moratoriumId)
                .Expand(x => x.ntt_breathingspacemoratorium_ntt_debtoreligibilityreview_MoratoriumId)
                .Expand(x => x.ntt_debtorid)
                .FindEntryAsync();

            await _client.For<Ntt_breathingspacedebtor>()
                  .Key(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid)
                  .Action("ntt_BSSDebtorSetMoratoriumTypeMH")
                  .Set(_auditContext.GenerateActionParameters())
                  .ExecuteAsync();

            var contentDict = new Dictionary<string, object>
            {
                { nameof(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid), moratorium.ntt_debtorid.Ntt_breathingspacedebtorid },
                { "ntt_BSSDebtorSetMoratoriumTypeMH", true }
            };
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return moratoriumId;
        }

        public async Task<Guid> SetBreathingSpaceAsStandard(Guid moratoriumId)
        {
            await _client.For<Ntt_breathingspacemoratorium>()
                  .Key(moratoriumId)
                  .Action("ntt_BSSMoratoriumSetTypeStandard")
                  .Set(_auditContext.GenerateActionParameters())
                  .ExecuteAsync();

            var contentDict = new Dictionary<string, object>
            {
                { nameof(moratoriumId), moratoriumId },
                { "ntt_BSSMoratoriumSetTypeStandard", true }
            };
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return moratoriumId;
        }

        public async Task<Guid> DebtorHideAddressAsync(DebtorHideAddressRequest model)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Expand(x => x.ntt_debtorid)
                .FindEntryAsync();

            if (moratorium == null)
            {
                throw new NotFoundHttpResponseException();
            }

            var bsStatus = moratorium.GetMoratoriumStatus();
            if (bsStatus != MoratoriumStatus.Active && bsStatus != MoratoriumStatus.Draft)
            {
                throw new ConflictHttpResponseException();
            }

            var result = await _client.For<Ntt_breathingspacedebtor>()
                .Key(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid)
                .Set(new
                {
                    ntt_addresswithheld = model.HideAddress
                })
                .UpdateEntryAsync(true);

            var contentDict = new Dictionary<string, object>
            {
                { nameof(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid), moratorium.ntt_debtorid.Ntt_breathingspacedebtorid },
                { nameof(model.HideAddress), model.HideAddress }
            };
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return result.Ntt_breathingspacedebtorid.Value;
        }

        public async Task<BusinessAdressResponse> AddDebtorBusinessAddressAsync(BusinessAddressRequest model)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.Address.OwnerId)
                .Expand(x => x.ntt_debtorid)
                .FindEntryAsync();

            var parameters = model
                .ToDictionary(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid.Value)
                .SetDynamicsActionAuditParameters(_auditContext);

            var result = await _client.ExecuteActionAsSingleAsync(
                "ntt_BSSBusinessCreate",
                parameters
            );

            var contentDict = model.ToDictionary(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid.Value);
            contentDict.Add("ntt_BSSBusinessCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return new BusinessAdressResponse
            {
                BusinessId = new Guid(result["BusinessId"].ToString()),
                AddressId = new Guid(result["AddressId"].ToString())
            };
        }

        public async Task UpdateBusinessNameAsync(BusinessNameUpdateRequest model)
        {
            await _client
               .For<Ntt_breathingspacemoratorium>()
               .Key(model.MoratoriumId)
               .Action("ntt_BSSAPIMABusinessUpdate")
               .Set(model.ToDictionary())
               .ExecuteAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.MoratoriumId), model.MoratoriumId);
            contentDict.Add("ntt_BSSAPIMABusinessUpdate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));
        }

        public async Task<NominatedContactCreateResponse> CreateNominatedContactAsync(NominatedContactCreateRequest model)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Expand(x => x.ntt_debtorid)
                .FindEntryAsync();

            var parameters = model
                .ToDictionary(_options, moratorium.ntt_debtorid.Ntt_breathingspacedebtorid.Value)
                .SetDynamicsActionAuditParameters(_auditContext);

            var result = await _client.ExecuteActionAsSingleAsync(
                "ntt_BSSPointofContactCreate",
                parameters
            );

            var contentDict = model.ToDictionary(_options, moratorium.ntt_debtorid.Ntt_breathingspacedebtorid.Value);
            contentDict.Add("ntt_BSSPointofContactCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return new NominatedContactCreateResponse
            {
                ContactId = new Guid(result["ContactId"].ToString()),
                RoleId = new Guid(result["RoleId"].ToString()),
                AddressId = new Guid(result["AddressId"].ToString()),
            };
        }

        public async Task UpdateNominatedContactAsync(NominatedContactUpdateRequest model)
        {
            _options.PointContactRoleSave.TryGetValue(((int)model.PointContactRole).ToString(), out var pointContactRoleId);

            var parameters = model.ToDictionary();
            parameters.Add("ContactRole", pointContactRoleId);

            await _client
                .For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Action("ntt_BSSAPIMAPointofContactUpdate")
                .Set(parameters)
                .ExecuteAsync();

            var contentDict = parameters;
            contentDict.Add(nameof(model.MoratoriumId), model.MoratoriumId);
            contentDict.Add("ntt_BSSAPIMAPointofContactUpdate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));
        }

        public async Task<DebtorNominatedContactResponse> GetNominatedContactAsync(Guid moratoriumId)
        {
            var contactRoles = await _client.For<Ntt_contactrole>()
                .Filter(x => x.ntt_BreathingSpaceMoratoriumid.ntt_breathingspacemoratoriumid == moratoriumId)
                .Expand(x => x.ntt_ExternalContactid)
                .FindEntriesAsync();

            var contactRole = contactRoles.OrderByDescending(x => x.createdon).FirstOrDefault();

            var externalContact = await _client.For<Ntt_externalcontact>()
                .Key(contactRole.ntt_ExternalContactid.ntt_externalcontactid)
                .Expand(x => x.ntt_externalcontact_inss_inssaddress)
                .FindEntryAsync();

            var contactAddress = externalContact.ntt_externalcontact_inss_inssaddress
                .OrderByDescending(x => x.Createdon)
                .FirstOrDefault();

            var model = new DebtorNominatedContactResponse
            {
                ContactId = contactRole.ntt_ExternalContactid.ntt_externalcontactid.Value,
                RoleId = contactRole.ntt_contactroleid.Value,
                PointContactRole = contactRole.GetMappedPointContactRoleId(_options),
                FullName = $"{externalContact.ntt_firstname} {externalContact.ntt_lastname}".Trim(),
                TelephoneNumber = externalContact.ntt_hometelephonenumber,
                EmailAddress = externalContact.ntt_emailaddress,
                NotificationMethod = externalContact.GetMappedPreferredChannelId(_options),
                CommunicationAddress = contactAddress != null ? new AddressResponse
                {
                    AddressId = contactAddress.inss_inssaddressid.Value,
                    AddressLine1 = contactAddress.inss_addressline1,
                    AddressLine2 = contactAddress.inss_addressline2,
                    TownCity = contactAddress.inss_addressline3,
                    County = contactAddress.inss_addressline4,
                    Country = contactAddress.inss_country,
                    Postcode = contactAddress.inss_postcode,
                } : null
            };

            return model;
        }

        public async Task DebtorSetContactPreference(DebtorContactPreferenceRequest model)
        {
            await _client
                .For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Action("ntt_BSSAPIMADebtorContactPreferenceUpdate")
                .Set(model.ToDictionary().SetDynamicsActionAuditParameters(_auditContext))
                .ExecuteAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.MoratoriumId), model.MoratoriumId);
            contentDict.Add("ntt_BSSAPIMADebtorContactPreferenceUpdate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));
        }

        public async Task<Guid> DebtorEndAccount(DebtorAccountEndRequest model)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Expand(x => x.ntt_debtorid)
                .Expand(x => x.ntt_breathingspacetypeid)
                .FindEntryAsync();

            var bsStatus = moratorium.GetMoratoriumStatus();
            if (bsStatus != MoratoriumStatus.Active)
            {
                throw new ConflictHttpResponseException();
            }

            var isInMentalHealthMoratorium = moratorium.ntt_breathingspacetypeid.ntt_breathingspacetypeid == _options.MentalHealthMoratoriumTypeId;

            var parameters = model.ToDictionary(_options, isInMentalHealthMoratorium)
                             .SetDynamicsActionAuditParameters(_auditContext);
            await _client
                .For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Action("ntt_BSSMoratoriumCancel")
                .Set(parameters)
                .ExecuteAsync();

            var contentDict = model.ToDictionary(_options, isInMentalHealthMoratorium);
            contentDict.Add(nameof(model.MoratoriumId), model.MoratoriumId);
            contentDict.Add("ntt_BSSMoratoriumCancel", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return model.MoratoriumId;
        }

        public async Task<BSCreationResponse> CreateBreathingSpace(CreateBreathingSpace moratorium, Guid organisationId)
        {
            var payload = moratorium.ToDictionary(_options, organisationId, _auditContext.GenerateActionParameters());
            var dynamicsResponse = await _client.ExecuteActionAsSingleAsync("ntt_BSSAPIMAMoratoriumCreate", payload);

            var result = JsonSerializer.Deserialize<BSCreationResponse>(dynamicsResponse["Response"].ToString());

            var contentDict = payload;
            contentDict.Add("ntt_BSSAPIMAMoratoriumCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return result;
        }

        public async Task<DebtDetailResponse> GetDebtDetail(Guid debtId)
        {
            var debt = await _client.For<Ntt_breathingspacedebt>()
                .Key(debtId)
                .ExpandDebt()
                .FindEntryAsync();

            var debtorDetail = debt.ToDebtDetail(_options);

            return debtorDetail;
        }

        public async Task<Guid> SubmitDebtEligibilityReview(DebtEligibilityReviewResponseRequest model)
        {
            var parameters = model.ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client
                .For<Ntt_breathingspacedebt>()
                .Key(model.DebtId)
                .Action(model.RemoveAfterReview.Value
                    ? "ntt_BSSMoneyAdviserRemoveDebtAfterReview"
                    : "ntt_BSSMoneyAdviserRetainDebtAfterReview")
                .Set(parameters)
                .ExecuteAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.DebtId), model.DebtId);
            contentDict.Add(model.RemoveAfterReview.Value
                    ? "ntt_BSSMoneyAdviserRemoveDebtAfterReview"
                    : "ntt_BSSMoneyAdviserRetainDebtAfterReview", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return model.DebtId;
        }

        public async Task<Guid> DebtorReviewClientEligibility(DebtorEligibilityReviewResponseRequest model)
        {
            var parameters = model.ToDictionary(_options)
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client
                .For<Ntt_breathingspacemoratorium>()
                .Key(model.MoratoriumId)
                .Action("ntt_BSSMoratoriumSetDebtorEligibilityReviewStatusAfterAdviserReview")
                .Set(parameters)
                .ExecuteAsync();

            var contentDict = model.ToDictionary(_options);
            contentDict.Add(nameof(model.MoratoriumId), model.MoratoriumId);
            contentDict.Add("ntt_BSSMoratoriumSetDebtorEligibilityReviewStatusAfterAdviserReview", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return model.MoratoriumId;
        }

        public async Task ConfirmDebtSold(Guid debtId)
        {
            await _client.For<Ntt_breathingspacedebt>()
               .Key(debtId)
               .Action("ntt_BSSDebtSoldConfirmation")
               .Set(_auditContext.GenerateActionParameters())
               .ExecuteAsSingleAsync();

            var contentDict = new Dictionary<string, object>();
            contentDict.Add(nameof(debtId), debtId);
            contentDict.Add("ntt_BSSDebtSoldConfirmation", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task RemoveDebt(RemoveDebtRequest removeDebtRequest)
        {
            var parameters = removeDebtRequest
                .ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacedebt>()
                .Key(removeDebtRequest.DebtId)
                .Action("ntt_BSSAPIMADebtMoneyAdviserRemove")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = removeDebtRequest.ToDictionary();
            contentDict.Add(nameof(removeDebtRequest.DebtId), removeDebtRequest.DebtId);
            contentDict.Add("ntt_BSSAPIMADebtMoneyAdviserRemove", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));
        }

        public async Task RemoveDebtPresubmission(Guid debtId, Guid organisationId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "MoneyAdviserId", organisationId.ToString() }
            };

            await _client.For<Ntt_breathingspacedebt>()
                .Key(debtId)
                .Action("ntt_BSSBreathingSpaceDebtMoneyAdviserDelete")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            parameters.Add("DebtId", debtId);
            parameters.Add("ntt_BSSBreathingSpaceDebtMoneyAdviserDelete", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(parameters));
        }

        public async Task MakeProposedDebtDetermination(ReviewProposedDebtRequest reviewProposedDebtRequest)
        {
            var parameters = reviewProposedDebtRequest
                .ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacedebt>()
                .Key(reviewProposedDebtRequest.DebtId)
                .Action(reviewProposedDebtRequest.AcceptProposedDebt.Value ? "ntt_BSSDebtAcceptProposed" : "ntt_BSSDebtRemoveProposed")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = reviewProposedDebtRequest.ToDictionary();
            contentDict.Add(nameof(reviewProposedDebtRequest.DebtId), reviewProposedDebtRequest.DebtId);
            contentDict.Add(reviewProposedDebtRequest.AcceptProposedDebt.Value ? "ntt_BSSDebtAcceptProposed" : "ntt_BSSDebtRemoveProposed", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task CreateDebtAndAdHocCreditor(CreateAdHocDebtRequest createAdHocDebtRequest)
        {
            var parameters = createAdHocDebtRequest
                             .ToDictionary()
                             .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacemoratorium>()
                .Key(createAdHocDebtRequest.MoratoriumId)
                .Action("ntt_BSSAdhocCreditorCreate_DebtCreate_WithStatusCheck")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = createAdHocDebtRequest.ToDictionary();
            contentDict.Add("ntt_BSSAdhocCreditorCreate_DebtCreate_WithStatusCheck", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task TransferDebtor(TransferDebtorRequest transferDebtorRequest)
        {
            var parameters = transferDebtorRequest
                .ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacemoratorium>()
                .Key(transferDebtorRequest.MoratoriumId)
                .Action("ntt_BSSMoratoriumTransferRequestCreate")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = transferDebtorRequest.ToDictionary();
            contentDict.Add(nameof(transferDebtorRequest.MoratoriumId), transferDebtorRequest.MoratoriumId);
            contentDict.Add("ntt_BSSMoratoriumTransferRequestCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task CompleteTransferDebtor(TransferDebtorCompleteRequest transferDebtorCompleteRequest)
        {
            var parameters = transferDebtorCompleteRequest
                .ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacemoratorium>()
                .Key(transferDebtorCompleteRequest.MoratoriumId)
                .Action("ntt_BSSMoratoriumTransferRequestComplete")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = transferDebtorCompleteRequest.ToDictionary();
            contentDict.Add(nameof(transferDebtorCompleteRequest.MoratoriumId), transferDebtorCompleteRequest.MoratoriumId);
            contentDict.Add("ntt_BSSMoratoriumTransferRequestComplete", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task AcknowledgeDebtorTransfer(DebtorTransferRequest debtorTransferRequest)
        {
            var parameters = debtorTransferRequest
                            .ToDictionary()
                            .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacemoratorium>()
                .Key(debtorTransferRequest.MoratoriumId)
                .Action("ntt_BSSMoratoriumTransferRequestAcknowledge")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = debtorTransferRequest.ToDictionary();
            contentDict.Add("ntt_BSSMoratoriumTransferRequestAcknowledge", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }
    }
}
