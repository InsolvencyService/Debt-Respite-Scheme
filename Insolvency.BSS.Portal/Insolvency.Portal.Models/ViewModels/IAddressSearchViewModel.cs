using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insolvency.Portal.Models.ViewModels
{
    public interface IAddressSearchViewModel
    {
        public Guid DebtId { get; set; }
        public Guid BusinessId { get; set; }
        public Guid AddressId { get; set; }
        public string ReturnAction { get; set; }
        public string Postcode { get; set; }
        public string SelectedAddress { get; set; }
        public string OnPostRedirectAction { get; set; }

        public IEnumerable<SelectListItem> Addresses { get; set; }
        void MapAddresses(AddressLookupResult lookupResult);
    }
}
