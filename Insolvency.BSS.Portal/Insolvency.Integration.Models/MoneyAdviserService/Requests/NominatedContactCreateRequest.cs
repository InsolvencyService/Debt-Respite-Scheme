using System;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class NominatedContactCreateRequest : AbstractDebtorNominatedContactAddress<NominatedContactAddress>
    {
        public virtual Guid MoratoriumId { get; set; }
    }
}