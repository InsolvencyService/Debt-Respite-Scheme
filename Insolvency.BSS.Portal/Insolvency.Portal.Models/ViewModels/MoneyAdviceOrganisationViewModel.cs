using System;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Portal.Models.ViewModels
{
    public class MoneyAdviceOrganisationViewModel
    {
        public MoneyAdviceOrganisationViewModel() { }
        public MoneyAdviceOrganisationViewModel(MoneyAdviserOrganizationResponse moneyAdviserOrganization)
        {
            Name = moneyAdviserOrganization.Name;
            Email = moneyAdviserOrganization.Email;
            Telephone = moneyAdviserOrganization.Telephone;
            Id = moneyAdviserOrganization.Id;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public bool ShouldDisplayName => !string.IsNullOrWhiteSpace(Name);
        public bool ShouldDisplayEmail => !string.IsNullOrWhiteSpace(Email);
        public bool ShouldDisplayTelephone => !string.IsNullOrWhiteSpace(Telephone);
        public bool ShouldDisplayMoneyOrganization => ShouldDisplayEmail || ShouldDisplayName || ShouldDisplayTelephone;
    }
}
