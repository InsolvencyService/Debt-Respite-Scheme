using System;

namespace Insolvency.Integration.Models.MoneyAdviserService.Responses
{
    public class NominatedContactCreateResponse
    {
        public Guid ContactId { get; set; }
        public Guid RoleId { get; set; }
        public Guid AddressId { get; set; }
    }
}
