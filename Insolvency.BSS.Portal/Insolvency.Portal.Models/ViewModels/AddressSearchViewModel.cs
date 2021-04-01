using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Insolvency.Common.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insolvency.Portal.Models.ViewModels
{
    public class AddressSearchViewModel : IAddressSearchViewModel, IConditionalRequiredValidation
    {
        [Required(ErrorMessage = "Enter postcode")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        public IEnumerable<SelectListItem> Addresses { get; set; }

        [Display(Name = "Choose address")]
        [ConditionalRequired(ErrorMessage = "Please select an address")]
        public string SelectedAddress { get; set; }

        public Guid AddressId { get; set; }
        public Guid DebtId { get; set; }
        public Guid BusinessId { get; set; }
        public string ReturnAction { get; set; }
        public bool IsEdit { get; set; }
        public string OnPostRedirectAction { get; set; }

        public bool ConditionalFlag => !String.IsNullOrWhiteSpace(Postcode);

        public virtual void MapAddresses(AddressLookupResult lookupResult)
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
