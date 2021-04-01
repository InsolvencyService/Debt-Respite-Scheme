using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Integration.Gateways.Audit;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models.CreditorService.Requests;
using Insolvency.Integration.Models.CreditorService.Responses;
using Microsoft.Extensions.Logging;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways
{
    public class CreditorServiceDynamicsGateway : ICreditorServiceDynamicsGateway
    {
        private readonly ILogger<CreditorServiceDynamicsGateway> _logger;
        private readonly IODataClient _client;
        private readonly AuditContext _auditContext;
        private readonly IAuditService _auditService;

        public CreditorServiceDynamicsGateway(
            IODataClient client,
            ILogger<CreditorServiceDynamicsGateway> logger,
            AuditContext auditContext,
            IAuditService auditService)
        {
            _logger = logger;
            _client = client;
            _auditContext = auditContext;
            _auditService = auditService;
        }

        public async Task DebtStopAllAction(Guid debtId, Guid creditorId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "CreditorId", creditorId.ToString() }
            };
            parameters.SetDynamicsActionAuditParameters(_auditContext);
            
            await _client.For<Ntt_breathingspacedebt>()
                .Key(debtId)
                .Action("ntt_BSSCreditorStopActionOnDebt")
                .Set(parameters)
                .ExecuteAsSingleAsync();

            var contentDict = parameters;
            contentDict.Add(nameof(debtId), debtId);
            contentDict.Add("ntt_BSSCreditorStopActionOnDebt", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return;
        }

        public async Task<ProposeDebtResponse> ProposeADebt(ProposeDebtRequest model)
        {
            var parameters = model
                .ToDictionary()
                .SetDynamicsActionAuditParameters(_auditContext);

            var result = await _client.ExecuteActionAsSingleAsync("ntt_BSSDebtCreate", parameters);
            var debtId = result["DebtId"].ToString();
            var createdOn = result["CreatedOn"].ToString().ToDateTimeOffset(Constants.UkDateTimeFormat);

            var contentDict = model.ToDictionary();
            contentDict.Add("ntt_BSSDebtCreate", true);
            await _auditService.PerformAuditing(_auditContext.ToAuditDetail(contentDict));

            return new ProposeDebtResponse
            {
                DebtId = new Guid(debtId),
                CreatedOn = createdOn,
            };
        }
    }
}
