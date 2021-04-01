using System;
using System.ComponentModel.DataAnnotations;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;

namespace Insolvency.Portal.Models.ViewModels
{
    public class DebtorAddBusinessViewModel
    {
        [Display(Name = "Enter the client's business or trading name")]
        [Required(ErrorMessage = "Please enter business or trading name")]
        [StringLength(100)]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Please select an option")]
        public string IsAddressSameAsCurrent { get; set; }

        public Address DebtorCurrentAddress { get; set; }

        public bool UseCurrentAddress => IsAddressSameAsCurrent == Option.Yes.ToString();

        public bool DisplayHideAddressLabel { get; set; }

        public Guid MoratoriumId { get; set; }
        public Guid BusinessId { get; set; }
        public string ReturnAction { get; set; }
        public bool IsEdit { get; set; }

        public BusinessAddressRequest ToCreateBusinessModel()
        {
            var address = DebtorCurrentAddress.ToCustomAddress();
            address.OwnerId = MoratoriumId;

            return new BusinessAddressRequest
            {
                Address = address,
                BusinessName = BusinessName
            };
        }       
    }
}
