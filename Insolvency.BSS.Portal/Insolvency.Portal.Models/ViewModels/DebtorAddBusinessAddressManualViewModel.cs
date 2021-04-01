using System;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAddBusinessAddressManualViewModel : AddressWithValidation
    {
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string ReturnAction { get; set; }
    }
}
