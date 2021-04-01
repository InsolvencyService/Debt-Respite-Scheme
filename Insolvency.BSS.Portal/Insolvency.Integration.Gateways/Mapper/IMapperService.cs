using System;
using Insolvency.Integration.Gateways.Entities;
using Insolvency.Integration.Gateways.MoratoriumEntities;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.Mapper
{
    public interface IMapperService
    {
        DebtorDetailsResponse MapDebtorDetailPublicData(BreathingSpaceResponse response);
        DebtorDetailsResponse MapDebtorNames(Ntt_breathingspacedebtor response);
        void SetDebtorDetailCurrentAddress(BreathingSpaceResponse response);
        void SetBusinessAddress(BusinessAddressResponse businessAddress, AddressResponse currentAddress, bool addressHidden);
        void SetDebtorDetailSensitiveData(BreathingSpaceResponse response, DynamicsGatewayOptions options);
        void SetAccountSearchResult(AccountSearchResponse response, Ntt_breathingspacedebtor debtor, Ntt_breathingspacemoratorium moratorium);
        AddressResponse MapCurrentAddressResponse(AddressResponse response);
        PreviousAddressResponse MapPreviousAddressResponse(PreviousAddressResponse response, bool includeToFrom = false);
        BreathingSpaceResponse BuildMoratorium(MoratoriumDetail response, DynamicsGatewayOptions options);
        void FilterMoratoriumByCreditor(BreathingSpaceResponse response, Guid creditorId);
    }
}
