using System.Collections.Generic;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class CreditorSearchResponse : List<CreditorSearchResult>
    {
        public CreditorSearchResponse() { }
        public CreditorSearchResponse(IEnumerable<CreditorSearchResult> list) : base(list)
        {
        }
    }
}
