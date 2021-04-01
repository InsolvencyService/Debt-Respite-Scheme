using System;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class ProposedCreditor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public CurrentAddress Address { get; set; }
    }
}