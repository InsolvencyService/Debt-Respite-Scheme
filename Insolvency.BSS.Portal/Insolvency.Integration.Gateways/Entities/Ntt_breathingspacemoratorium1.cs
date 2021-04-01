using System;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Ntt_breathingspacemoratorium : Abstract_Ntt_breathingspacemoratorium
    {
        public Guid? ntt_breathingspacemoratoriumid { get; set; }
        public override Guid GetId() => ntt_breathingspacemoratoriumid.Value;
    }
}
