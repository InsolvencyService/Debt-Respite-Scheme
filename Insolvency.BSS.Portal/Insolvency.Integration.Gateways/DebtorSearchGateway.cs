using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Insolvency.Common.Enums;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Gateways.Mapper;
using Insolvency.Integration.Gateways.OData;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways
{
    public class DebtorSearchGateway : IDebtorSearchGateway
    {
        private readonly IMapperService _mapperService;
        public string Surname { get; set; }
        public string Reference { get; set; }
        public bool IsRefValid { get; set; }
        public bool IsSurnameValid { get; set; }
        public bool IsDobValid { get; set; }
        public DateTime DateOfBirth { get; set; }

        public IODataClient Client { get; }

        public DebtorSearchGateway(IODataClient client, IMapperService mapperService)
        {
            Client = client;
            _mapperService = mapperService ?? throw new ArgumentNullException(nameof(mapperService));
        }

        public async Task<IEnumerable<AccountSearchResponse>> SearchAccountsAsync(AccountSearchBaseRequest searchParam, Guid organisationId)
        {
            Setup(searchParam);

            if (!IsSearchValid)
                return Enumerable.Empty<AccountSearchResponse>();

            var debtors = await Client.For<Ntt_breathingspacedebtor>()
                .Expand(x => x.ntt_breathingspacedebtor_inss_inssaddress)
                .Expand(x => x.Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid)
                .Filter(GetSearchQuery())
                .OrderBy(x => x.ntt_lastname)
                .FindEntriesAsync();

            debtors = debtors.ToList();

            if (!debtors.Any())
                return Enumerable.Empty<AccountSearchResponse>();

            var moratoriums = await Client.For<__Ntt_breathingspacemoratorium>()
                .Expand(m => m.ntt_breathingspacetypeid)
                .Expand(m => m.ntt_ManagingMoneyAdviserOrganisationId)
                .WhereInFilter(x => x.ntt_breathingspacemoratoriumid, debtors.SelectMany(d => d.GetMoratoriumIds()).ToList())
                .FindEntriesAsync();

            var moratoriumsByDebtorId = moratoriums
                .GroupBy(x => x._ntt_debtorid_value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var indexDebtors = debtors.ToDictionary(x => x.GetId(), x => x);

            moratoriumsByDebtorId.ToList()
                .ForEach(x => indexDebtors[x.Key.Value].Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid = x.Value.Select(y => y.MapToDynamicOriginal()).ToList());

            var results = debtors
               .Where(x => x.Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid
               .Any(y => y._ntt_managingmoneyadviserorganisationid_value == organisationId 
                         || y.GetMoratoriumStatus() != MoratoriumStatus.Draft))
               .SelectMany(
                    (debtor) => debtor.Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid,
                    (debtor, moratorium) =>
                    {
                        var searchResult = new AccountSearchResponse();
                        var isAddressHidden = debtor.Ntt_addresswithheld ?? false;
                        var isManagingOrganisation = moratorium._ntt_managingmoneyadviserorganisationid_value == organisationId;

                        _mapperService.SetAccountSearchResult(searchResult, debtor, moratorium);

                        if (!isManagingOrganisation && isAddressHidden)
                            searchResult.Address = null;

                        return searchResult;
                    })
               .ToList();

            return results;
        }

        protected virtual bool IsSearchValid => IsRefValid || IsSurnameValid || IsDobValid;

        protected virtual Expression<Func<Ntt_breathingspacedebtor, bool>> GetSearchQuery()
        {
            var startDay = new DateTimeOffset(DateOfBirth);
            var endDay = startDay.AddDays(1);

            if (IsRefValid)
            {
                return x => x.Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid
                        .Any(y => y.ntt_referencenumber == Reference);
            }

            if (IsSurnameValid && IsDobValid)
            {
                return x => x.ntt_lastname == Surname
                              && x.ntt_dateofbirth >= startDay && x.ntt_dateofbirth < endDay;
            }

            if (IsSurnameValid)
            {
                return x => x.ntt_lastname == Surname;
            }

            return x => x.ntt_dateofbirth >= startDay && x.ntt_dateofbirth < endDay;
        }

        protected virtual void Setup(AccountSearchBaseRequest accountSearch)
        {
            Surname = accountSearch.Surname;
            DateOfBirth = accountSearch.DateOfBirth;
            Reference = accountSearch.BreathingSpaceReference;

            IsRefValid = !string.IsNullOrEmpty(Reference);
            IsSurnameValid = !string.IsNullOrEmpty(Surname);
            IsDobValid = DateOfBirth.Date != DateTime.Today.Date
                          && DateOfBirth != default;
        }
    }
}
