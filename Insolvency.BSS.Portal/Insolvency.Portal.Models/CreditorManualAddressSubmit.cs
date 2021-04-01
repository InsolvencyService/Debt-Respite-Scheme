using System;

namespace Insolvency.Portal.Models
{
    public class CreditorManualAddressSubmit : AddressWithValidation
    {
        public Guid DebtId { get; set; }
    }
}