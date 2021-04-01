using System;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class ClientDetailsUpdateRequest : ClientDetailsCreateRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }        
    }
}
