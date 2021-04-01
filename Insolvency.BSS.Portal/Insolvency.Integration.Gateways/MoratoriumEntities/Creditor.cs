using System;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class Creditor
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid TypeId { get; set; }
        public CurrentAddress Address { get; set; }
    }
}