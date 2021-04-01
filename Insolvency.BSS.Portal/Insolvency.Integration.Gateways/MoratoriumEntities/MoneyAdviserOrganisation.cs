using System;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class MoneyAdviserOrganisation
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public MoneyAdviserOrganizationResponse ToMoneyAdviserOrganizationResponse()
        {
            return new MoneyAdviserOrganizationResponse
            {
                Email = Email,
                Name = Name,
                Telephone = Telephone,
                Id = Id,
            };
        }
    }
}