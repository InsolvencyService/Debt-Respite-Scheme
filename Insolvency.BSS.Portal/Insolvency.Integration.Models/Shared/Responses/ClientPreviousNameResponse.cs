using System;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class ClientPreviousNameResponse
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public Guid Id { get; set; }
    }
}
