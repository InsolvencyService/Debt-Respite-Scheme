using System;

namespace Insolvency.Integration.Models.Shared.Requests
{
    public class ClientPreviousNameCreateRequest
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Guid MoratoriumId { get; set; }
    }
}
