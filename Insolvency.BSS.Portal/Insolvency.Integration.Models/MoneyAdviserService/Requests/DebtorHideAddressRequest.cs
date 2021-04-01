using System;
using System.Text.Json.Serialization;

namespace Insolvency.Integration.Models.MoneyAdviserService.Requests
{
    public class DebtorHideAddressRequest
    {
        [JsonIgnore]
        public Guid MoratoriumId { get; set; }
        public bool HideAddress { get; set; }
    }
}