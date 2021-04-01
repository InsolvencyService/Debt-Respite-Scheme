using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Common.Enums;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways
{
    public class BreathingSpaceBrowseGateway : IBreathingSpaceBrowseGateway
    {
        private readonly IODataClient _client;
        private readonly DynamicsGatewayOptions _options;
        private readonly Dictionary<BreathingSpaceBrowseTask, Func<Guid, bool, Task<BreathingSpaceBrowseResponse>>> _taskBrowseFunctions;

        public BreathingSpaceBrowseGateway(IODataClient client,
            DynamicsGatewayOptions options)
        {
            _client = client;
            _options = options;
            _taskBrowseFunctions = new Dictionary<BreathingSpaceBrowseTask, Func<Guid, bool, Task<BreathingSpaceBrowseResponse>>>
            {
                { BreathingSpaceBrowseTask.DebtsToBeReviewed, BrowseDebtsToBeReviewedAsync },
                { BreathingSpaceBrowseTask.ClientsToBeReviewed, BrowseClientsToBeReviewedAsync },
                { BreathingSpaceBrowseTask.NewDebtsProposed, BrowseNewDebtsProposedAsync },
                { BreathingSpaceBrowseTask.DebtsToBeTransferred, BrowseDebtsToBeTransferredAsync },
                { BreathingSpaceBrowseTask.ClientsToBeTransferred, BrowseClientsToBeTransferred },
                { BreathingSpaceBrowseTask.ClientsTransferredToMoneyAdviser, BrowseClientsTransferredToMoneyAdviser }
            };
        }

        public async Task<BreathingSpaceBrowseResponse> BrowserBreathingSpaceByAsync(BreathingSpaceBrowseCategory category, BreathingSpaceBrowseTask? task, Guid moneyAdviserId, bool showNewestFirst)
        {
            if (category == BreathingSpaceBrowseCategory.ActiveBreathingSpaces)
            {
                return await BrowseActiveBreathingSpacesAsync(moneyAdviserId, showNewestFirst);
            }

            if (category == BreathingSpaceBrowseCategory.TasksToDo)
            {
                return await _taskBrowseFunctions[task.Value](moneyAdviserId, showNewestFirst);
            }

            if (category == BreathingSpaceBrowseCategory.SentToMoneyAdviser)
            {
                return await BrowseSentToMoneyAdviserAsync(moneyAdviserId, showNewestFirst);
            }

            return await EmptyResult(moneyAdviserId, showNewestFirst);
        }

        public BreathingSpaceBrowseItem MapBreathingSpaceBrowseResult(Ntt_breathingspacemoratorium entity)
        {
            return new BreathingSpaceBrowseItem()
            {
                BreathingSpaceReference = entity.ntt_referencenumber,
                DateStarted = entity.ntt_commencementdate,
                DateEnded = entity.ntt_expirydate,
                FirstName = entity.ntt_debtorid.ntt_firstname,
                LastName = entity.ntt_debtorid.ntt_lastname,
                BreathingSpaceId = entity.GetId()
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseActiveBreathingSpacesAsync(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();

            var command = _client.For<Ntt_breathingspacemoratorium>()
                .Expand(m => m.ntt_debtorid)
                .Filter(m => m._ntt_breathingspacestatusid_value == MoratoriumIdStatusMap.Active)
                .Filter(m => m._ntt_managingmoneyadviserorganisationid_value == moneyAdviserId);

            command = showNewestFirst ?
                command.OrderByDescending(m => m.ntt_commencementdate) : command.OrderBy(m => m.ntt_commencementdate);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseDebtsToBeReviewedAsync(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();
            var reviewRequested = _options.DebtEligibilityReviewStatusId(DebtEligibilityReviewStatus.ReviewRequested);

            var command = _client.For<ntt_debteligibilityreview>()
                 .Expand(d => d.ntt_DebtId.ntt_BreathingSpaceMoratoriumId)
                 .Expand(d => d.ntt_DebtId.ntt_BreathingSpaceMoratoriumId.ntt_debtorid)
                 .Filter(d => d._ntt_eligibilitystatusid_value == reviewRequested)
                 .Filter(d => d.ntt_DebtId._ntt_managingmoneyadviserorganisationid_value == moneyAdviserId);

            command = showNewestFirst ?
                command.OrderByDescending(m => m.createdon) : command.OrderBy(m => m.createdon);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_DebtId.ntt_BreathingSpaceMoratoriumId)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseClientsToBeReviewedAsync(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();
            var reviewRequestedStatus = _options.DebtorEligibilityReviewStatusId(DebtorEligibilityReviewStatus.ReviewRequested);

            var command = _client.For<ntt_debtoreligibilityreview>()
                 .Expand(d => d.ntt_MoratoriumId)
                 .Expand(d => d.ntt_MoratoriumId.ntt_debtorid)
                 .Filter(d => d._ntt_eligibilitystatusid_value == reviewRequestedStatus)
                 .Filter(d => d.ntt_MoratoriumId._ntt_managingmoneyadviserorganisationid_value == moneyAdviserId);

            command = showNewestFirst ?
                command.OrderByDescending(m => m.createdon) : command.OrderBy(m => m.createdon);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_MoratoriumId)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseNewDebtsProposedAsync(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();
            var proposedDebtStatus = Ntt_breathingspacedebt.GetDebtStatusValue(_options, DebtStatus.Draft_CreditorProposedNewDebt);

            var command = _client.For<Ntt_breathingspacedebt>()
                 .Expand(d => d.ntt_BreathingSpaceMoratoriumId)
                 .Expand(d => d.ntt_BreathingSpaceMoratoriumId.ntt_debtorid)
                 .Filter(d => d._ntt_debtstatusid_value == proposedDebtStatus)
                 .Filter(d => d.ntt_BreathingSpaceMoratoriumId._ntt_managingmoneyadviserorganisationid_value == moneyAdviserId);

            command = showNewestFirst ?
                command.OrderByDescending(m => m.createdon) : command.OrderBy(m => m.createdon);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_BreathingSpaceMoratoriumId)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseDebtsToBeTransferredAsync(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();
            var soldStatus = Ntt_breathingspacedebt.GetDebtStatusValue(_options, DebtStatus.Active_SoldOnDebt);

            var command = _client.For<Ntt_breathingspacedebt>()
                 .Expand(d => d.ntt_BreathingSpaceMoratoriumId)
                 .Expand(d => d.ntt_BreathingSpaceMoratoriumId.ntt_debtorid)
                 .Filter(d => d._ntt_debtstatusid_value == soldStatus)
                 .Filter(d => d.ntt_BreathingSpaceMoratoriumId._ntt_managingmoneyadviserorganisationid_value == moneyAdviserId);

            command = showNewestFirst ?
                command.OrderByDescending(m => m.ntt_commencementdate) : command.OrderBy(m => m.ntt_commencementdate);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_BreathingSpaceMoratoriumId)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseClientsToBeTransferred(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();

            var command = _client.For<ntt_moratoriumtransferrequest>()
                 .Expand(r => r.ntt_breathingspacemoratoriumid)
                 .Expand(r => r.ntt_breathingspacemoratoriumid.ntt_debtorid)
                 .Expand(r => r.ntt_owningmoneyadviceorganisationid)
                 .Filter(r => r.ntt_owningmoneyadviceorganisationid.inss_moneyadviserorganisationid == moneyAdviserId)
                 .Filter(r => r.statuscode == (int)TransferDebtorRequestStatusCodes.Requested);

            command = showNewestFirst ?
                command.OrderByDescending(r => r.ntt_requestedon) : command.OrderBy(r => r.ntt_requestedon);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_breathingspacemoratoriumid)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseClientsTransferredToMoneyAdviser(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();

            var command = _client.For<ntt_moratoriumtransferrequest>()
                 .Expand(r => r.ntt_breathingspacemoratoriumid)
                 .Expand(r => r.ntt_breathingspacemoratoriumid.ntt_debtorid)
                 .Expand(r => r.ntt_requestingmoneyadviceorganisationid)
                 .Filter(r => r.ntt_requestingmoneyadviceorganisationid.inss_moneyadviserorganisationid == moneyAdviserId)
                 .Filter(r => r.statuscode == (int)TransferDebtorRequestStatusCodes.Transferred);

            command = showNewestFirst ?
                command.OrderByDescending(r => r.ntt_completedon) : command.OrderBy(r => r.ntt_completedon);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_breathingspacemoratoriumid)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual async Task<BreathingSpaceBrowseResponse> BrowseSentToMoneyAdviserAsync(Guid moneyAdviserId, bool showNewestFirst)
        {
            var annotations = new ODataFeedAnnotations();

            var command = _client.For<ntt_moratoriumtransferrequest>()
                 .Expand(r => r.ntt_breathingspacemoratoriumid)
                 .Expand(r => r.ntt_breathingspacemoratoriumid.ntt_debtorid)
                 .Expand(r => r.ntt_owningmoneyadviceorganisationid)
                 .Filter(r => r.ntt_owningmoneyadviceorganisationid.inss_moneyadviserorganisationid == moneyAdviserId)
                 .Filter(r => r.statuscode == (int)TransferDebtorRequestStatusCodes.Transferred);

            command = showNewestFirst ?
                command.OrderByDescending(r => r.ntt_completedon) : command.OrderBy(r => r.ntt_completedon);
            var list = (await command.FindEntriesAsync(annotations))
                .Select(m => MapBreathingSpaceBrowseResult(m.ntt_breathingspacemoratoriumid)).ToList();

            return new BreathingSpaceBrowseResponse
            {
                BreathingSpaceBrowseItems = list
            };
        }

        protected virtual Task<BreathingSpaceBrowseResponse> EmptyResult(Guid moneyAdviserId, bool showNewestFirst)
        {
            return Task.FromResult(
                new BreathingSpaceBrowseResponse
                {
                    BreathingSpaceBrowseItems = new List<BreathingSpaceBrowseItem>()
                });
        }
    }
}
