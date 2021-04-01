using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorPostcodeViewModel
    {
        [Required(ErrorMessage = "Enter postcode")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }
        public IEnumerable<SelectListItem> Addresses { get; set; }

        [Display(Name = "Choose address")]
        public string SelectedAddress { get; set; }

        public void MapAddresses(AddressLookupResult lookupResult)
        {
            var result = new List<SelectListItem>();
            this.Addresses = result;
            if (lookupResult.Addresses == null || lookupResult.Addresses.Count() == 0)
            {
                result.Add(new SelectListItem("No addresses found", "", true, true));
                return;
            }
            result.Add(new SelectListItem($"{lookupResult.AddressesFound} addresses found", "", true, true));
            result.AddRange(lookupResult.Addresses.Select(x => new SelectListItem(x.Address, x.Id)));
        }
    }
}
