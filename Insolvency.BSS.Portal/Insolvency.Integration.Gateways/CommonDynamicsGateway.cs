using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Insolvency.Common.Enums;
using Insolvency.Common.Exceptions;
using Insolvency.Integration.Gateways.Audit;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Gateways.Mapper;
using Insolvency.Integration.Gateways.MoratoriumEntities;
using Insolvency.Integration.Gateways.OData;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Requests;
using Insolvency.Integration.Models.Shared.Responses;
using Microsoft.Extensions.Logging;
using Simple.OData.Client;
using Creditor = Insolvency.Integration.Models.Creditor;

namespace Insolvency.Integration.Gateways
{
    public class CommonDynamicsGateway : ICommonDynamicsGateway
    {
        private readonly IODataClient _client;

        private readonly DynamicsGatewayOptions _options;
        private readonly ILogger<CommonDynamicsGateway> _logger;
        private readonly IMapperService _mapperService;
        private readonly AuditContext _auditContext;
        private readonly IAuditService _auditService;

        public CommonDynamicsGateway(
            IODataClient client,
            DynamicsGatewayOptions options,
            ILogger<CommonDynamicsGateway> logger,
            IMapperService mapperService,
            AuditContext auditContext,
            IAuditService auditService)
        {
            _logger = logger;
            _client = client;
            _options = options;
            _mapperService = mapperService;
            _auditContext = auditContext;
            _auditService = auditService;
        }

        public async Task DebtSoldOn(DebtSoldOnRequest model)
        {
            var parameters = model.ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacedebt>()
               .Key(model.DebtId)
               .Action("ntt_BSSAPIMADebtSoldProposal")
               .Set(parameters)
               .ExecuteAsSingleAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.DebtId), model.DebtId);
            contentDict.Add("ntt_BSSDebtSoldProposal", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task CreateDebtorEligibilityReviewRequest(DebtorEligibilityReviewRequest model)
        {
            var parameters = model.ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacemoratorium>()
               .Key(model.MoratoriumId)
               .Action("ntt_BSSAPIMADebtorEligibilityReviewCreate")
               .Set(parameters)
               .ExecuteAsSingleAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.MoratoriumId), model.MoratoriumId);
            contentDict.Add("ntt_BSSAPIMADebtorEligibilityReviewCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task CreateDebtEligibilityReviewRequest(DebtEligibilityReviewRequest model)
        {
            var parameters = model.ToDictionary(_options)
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacedebt>()
               .Key(model.DebtId)
               .Action("ntt_BSSAPIMADebtEligibilityReviewCreate")
               .Set(parameters)
               .ExecuteAsSingleAsync();

            var contentDict = model.ToDictionary(_options);
            contentDict.Add("ntt_BSSDebtEligibilityReviewCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task<IEnumerable<CreditorSearchDetailedResponse>> GetAllCMPCreditors()
        {
            var debtTypesResult = await GetAllRecordsAsync(_client.For<INSS_debttype>());
            var debtTypes = debtTypesResult.ToDictionary(x => x.inss_debttypeid, x => x);

            var query = _client
                .For<Inss_cmpcreditor>()
                .Expand(x => x.inss_cmpcreditor_inss_creditordebttype_CMPCreditorId)
                .Expand(x => x.ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId);

            var result = await GetAllRecordsAsync(query);

            var models = result
                .Where(x => x.ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId.Count == 1)
                .Select(x => new CreditorSearchDetailedResponse
                {
                    Id = x.ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId.First().GetId(),
                    Name = x.inss_name,
                    IsGovermentCreditor = x._inss_creditortypeid_value == _options.GovermentCreditorId,
                    ModifiedOn = x.modifiedon.Value,
                    DebtTypes = x.inss_cmpcreditor_inss_creditordebttype_CMPCreditorId.Select(y => new DebtType { Id = y._inss_debttypeid_value, Name = debtTypes[y._inss_debttypeid_value.Value].inss_name }).ToList()
                })
                .ToList();

            return models;
        }

        protected virtual async Task<IEnumerable<T>> GetAllRecordsAsync<T>(IBoundClient<T> query) where T : class
        {
            var annotations = new ODataFeedAnnotations();
            var entities = await query.FindEntriesAsync(annotations);
            var result = entities.ToList();

            while (annotations.NextPageLink != null)
            {
                entities = await query.FindEntriesAsync(annotations.NextPageLink, annotations);
                result.AddRange(entities);
            }

            return result;
        }

        public async Task<DebtorDetailsResponse> GetClientNamesAsync(Guid moratoriumId)
        {
            var moratorium = await _client.For<Ntt_breathingspacemoratorium>()
            .Key(moratoriumId)
            .Expand(x => x.ntt_debtorid)
            .FindEntryAsync();

            var debtor = await _client.For<Ntt_breathingspacedebtor>()
                .Key(moratorium.ntt_debtorid.Ntt_breathingspacedebtorid)
                .Expand(x => x.ntt_breathingspacedebtor_ntt_previousname_DebtorId)
                .FindEntryWithLogsAsync(_logger);

            return _mapperService.MapDebtorNames(debtor);
        }

        public async Task<BreathingSpaceResponse> GetMaBreathingSpaceAsync(Guid moratoriumId, Guid organisationId)
        {
            var bsResponse = await GetMoratoriumAsync(moratoriumId);

            if (bsResponse is null)
            {
                return null;
            }

            var shouldIncludeSensitiveData = bsResponse.OrganisationId == organisationId;

            if (shouldIncludeSensitiveData)
                return bsResponse;

            if (bsResponse.DebtorDetails.MoratoriumStatus == MoratoriumStatus.Draft)
            {
                return null;
            }

            bsResponse.DebtorEligibilityReviews = null;
            bsResponse.DebtDetails = null;
            bsResponse.PreviousAddresses = null;

            bsResponse.DebtorDetails = _mapperService.MapDebtorDetailPublicData(bsResponse);

            foreach (var businessAddress in bsResponse.DebtorBusinessDetails)
            {
                _mapperService.SetBusinessAddress(businessAddress, bsResponse.CurrentAddress, bsResponse.DebtorDetails.AddressHidden);
            }
            _mapperService.SetDebtorDetailCurrentAddress(bsResponse);

            return bsResponse;
        }

        public async Task<BreathingSpaceResponse> GetCreditorBreathingSpaceAsync(Guid moratoriumId, Guid organisationId)
        {
            var bsResponse = await GetMoratoriumAsync(moratoriumId);

            if (bsResponse is null)
            {
                return null;
            }

            _mapperService.FilterMoratoriumByCreditor(bsResponse, organisationId);

            if (!bsResponse.DebtDetails.Any())
            {
                throw new UnauthorizedHttpResponseException();
            }

            return bsResponse;
        }

        private async Task<BreathingSpaceResponse> GetMoratoriumAsync(Guid moratoriumId)
        {
            var moratoriumResponse = await _client.For<Ntt_breathingspacemoratorium>()
                .Key(moratoriumId)
                .Action("ntt_BSSMoratoriumGet")
                .ExecuteAsScalarWithLogsAsync<string, Ntt_breathingspacemoratorium>(_logger);

            var response = JsonSerializer.Deserialize<MoratoriumDetail>(moratoriumResponse);

            if (response is null)
            {
                return null;
            }

            return _mapperService.BuildMoratorium(response, _options);
        }

        public async Task<Guid> CreateAdHocCreditor(string name)
        {
            var adHocCreditor = new ntt_breathingspaceadhoccreditor
            {
                ntt_name = name
            };

            var adHocResult = await _client.For<ntt_breathingspaceadhoccreditor>()
                .Set(adHocCreditor)
                .Expand(x => x.ntt_breathingspaceadhoccreditor_ntt_breathingspacecreditor_adhoccreditorid)
                .InsertEntryWithLogsAsync(_logger);

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(adHocCreditor.ToAuditDictionary()));

            return adHocResult
                .ntt_breathingspaceadhoccreditor_ntt_breathingspacecreditor_adhoccreditorid
                .First().ntt_breathingspacecreditorid.Value;
        }

        public async Task<Creditor> GetGenericCreditorById(Guid creditorId)
        {
            var genericCreditor = await _client.For<Ntt_breathingspacecreditor>()
                .Expand(x => x.ntt_adhoccreditorid)
                .Expand(x => x.ntt_CMPCreditorId)
                .Expand(x => x.ntt_NotificationAddressId)
                .Key(creditorId)
                .FindEntryAsync();

            var address = genericCreditor.ntt_NotificationAddressId;

            if (genericCreditor.ntt_CMPCreditorId != null)
            {
                var cmpCreditor = await _client.For<Inss_cmpcreditor>()
                    .Key(genericCreditor.ntt_CMPCreditorId.inss_cmpcreditorid)
                    .Expand(x => x.inss_cmpcreditor_inss_creditordebttype_CMPCreditorId)
                    .FindEntryAsync();

                var debtTypeIds = cmpCreditor.GetDebtTypesIds();

                var innsDebtTypes = await _client
                    .For<INSS_debttype>()
                    .WhereInFilter(x => x.inss_debttypeid, debtTypeIds)
                    .FindEntriesAsync();

                var mappedDebtTypes = innsDebtTypes
                    .Select(x => x.MapTopDebtType())
                    .ToList();

                var isGovCreditor = cmpCreditor._inss_creditortypeid_value.Value == _options.GovermentCreditorId;

                return new Creditor
                {
                    Id = creditorId,
                    Name = cmpCreditor.inss_name,
                    IsGovermentCreditor = isGovCreditor,
                    DebtTypes = mappedDebtTypes,
                    AddressId = address?.GetId() ?? default,
                    AddressLine1 = address?.inss_addressline1,
                    AddressLine2 = address?.inss_addressline2,
                    County = address?.inss_addressline4,
                    TownCity = address?.inss_addressline3,
                    PostCode = address?.inss_postcode,
                    Country = address?.inss_country
                };
            }
            if (genericCreditor.ntt_adhoccreditorid != null)
            {
                var adHocCreditor = await _client.For<ntt_breathingspaceadhoccreditor>()
                   .Key(genericCreditor.ntt_adhoccreditorid.ntt_breathingspaceadhoccreditorid)
                   .FindEntryAsync();

                return new Creditor
                {
                    Id = creditorId,
                    Name = genericCreditor.ntt_adhoccreditorid.ntt_name,
                    AddressId = address?.GetId() ?? default,
                    AddressLine1 = address?.inss_addressline1,
                    AddressLine2 = address?.inss_addressline2,
                    County = address?.inss_addressline4,
                    TownCity = address?.inss_addressline3,
                    PostCode = address?.inss_postcode,
                    IsGovermentCreditor = false,
                    Country = address?.inss_country
                };
            }

            return null;
        }

        public async Task<CreditorSearchResponse> SearchCmpCreditors(string creditorName)
        {
            var cmpCreditorResult = await _client.For<Inss_cmpcreditor>()
                .Expand(x => x.ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId)
                .Expand(x => x.inss_cmpcreditor_inss_creditordebttype_CMPCreditorId)
                .Filter(c => c.inss_name.Contains(creditorName))
                .FindEntriesAsync();

            var searchResult = cmpCreditorResult
                .Where(x => x.ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId.Any() && x.inss_cmpcreditor_inss_creditordebttype_CMPCreditorId.Any())
                .Select(x => new CreditorSearchResult
                {
                    Id = x.ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId.First().ntt_breathingspacecreditorid.Value,
                    Name = x.inss_name
                });

            return new CreditorSearchResponse(searchResult);
        }

        public async Task<Guid> AddAdHocCreditorAddressAsync(CustomAddress model)
        {
            var dynamicsModel = new Inss_InssAddress
            {
                inss_addressline1 = model.AddressLine1,
                inss_addressline2 = model.AddressLine2,
                inss_postcode = model.Postcode,
                inss_addressline4 = model.County,
                inss_addressline3 = model.TownCity,
                inss_country = model.Country
            };
            var result = await _client.For<Inss_InssAddress>()
                .Set(dynamicsModel)
                .InsertEntryWithLogsAsync(_logger);

            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(result.ToAuditDictionary()));

            await _client.For<Ntt_breathingspacecreditor>()
              .Key(model.OwnerId)
              .LinkEntryWithLogsAsync(_logger, result, "ntt_NotificationAddressId");

            return result.inss_inssaddressid.Value;
        }

        public async Task TransferDebtToNewAdhocCreditor(DebtSoldOnToAdHocCreditorRequest model)
        {
            var parameters = model.ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            await _client.For<Ntt_breathingspacedebt>()
               .Key(model.DebtId)
               .Action("ntt_BSSAdhocCreditorCreate_DebtSoldProposal")
               .Set(parameters)
               .ExecuteAsSingleAsync();

            var contentDict = model.ToDictionary();
            contentDict.Add(nameof(model.DebtId), model.DebtId);
            contentDict.Add("ntt_BSSAdhocCreditorCreate_DebtSoldProposal", true);
            var auditDetail = _auditContext.ToAuditDetail(contentDict);
            await _auditService.PerformAuditing(auditDetail);

            return;
        }

        public async Task<bool> ExpireMoratoriumAsync()
        {
            var result = await _client.ExecuteActionAsSingleAsync(
                "ntt_BSSMoratoriumBulkUpdatetoExpiredStatus",
                new Dictionary<string, object>()
            );

            var expirePending = (bool)result["MoreRecordsToProcess"];

            return expirePending;
        }
    }
}
