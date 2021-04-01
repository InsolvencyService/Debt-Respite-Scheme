using System;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class ExternalContact
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string HomeTelephone { get; set; }
        public string EmailAddress { get; set; }
        public string PreferredChannelText { get; set; }
        public int PreferredChannel { get; set; }
        public CurrentAddress CurrentAddress { get; set; }
    }
}