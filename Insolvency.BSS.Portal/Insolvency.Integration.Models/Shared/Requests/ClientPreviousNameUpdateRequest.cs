using System;

namespace Insolvency.Integration.Models.Shared.Requests
{
    public class ClientPreviousNameUpdateRequest : ClientPreviousNameCreateRequest
    {
        public Guid NameId { get; set; }
    }
}
