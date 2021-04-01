using System;
using System.Text.Json;

namespace Insolvency.Integration.Gateways.Entities
{
    public class __Ntt_breathingspacemoratorium : Abstract_Ntt_breathingspacemoratorium
    {
        public Guid ntt_breathingspacemoratoriumid { get; set; }
        public override Guid GetId() => ntt_breathingspacemoratoriumid;

        public Ntt_breathingspacemoratorium MapToDynamicOriginal()
        {
            var __moratorium = JsonSerializer.Serialize(this);
            var result = JsonSerializer.Deserialize<Ntt_breathingspacemoratorium>(__moratorium);
            return result;
        }
    }
}
