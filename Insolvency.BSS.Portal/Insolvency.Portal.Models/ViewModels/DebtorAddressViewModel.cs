using System.Collections.Generic;
using System.Linq;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAddressViewModel
    {
        public DebtorAddressViewModel()
        {
        }

        public DebtorAddressViewModel(BreathingSpaceResponse accountSummary)
        {
            if (accountSummary.CurrentAddress != null)
            {
                CurrentAddress = new Address(accountSummary.CurrentAddress);
            }
            PreviousAddresses = accountSummary.PreviousAddresses?
                            .Select(add => new Address(add)) ?? Enumerable.Empty<Address>()
                            .ToList();
            AddressHidden = accountSummary.DebtorDetails.AddressHidden;
        }

        public Address CurrentAddress { get; set; }
        public IEnumerable<Address> PreviousAddresses { get; set; }
        public bool AddressHidden { get; set; }
        public string ReturnAction { get; set; }
        public bool ShouldDisplayAddress => !AddressHidden && !string.IsNullOrWhiteSpace(CurrentAddress.ToSingleLineString());
    }
}