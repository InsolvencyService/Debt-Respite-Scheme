using System;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class PreviousName
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string CreatedOn { get; set; }

        public ClientPreviousNameResponse ToPreviousName()
        {
            return new ClientPreviousNameResponse
            {
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Id = new Guid(Id)
            };
        }
    }
}
