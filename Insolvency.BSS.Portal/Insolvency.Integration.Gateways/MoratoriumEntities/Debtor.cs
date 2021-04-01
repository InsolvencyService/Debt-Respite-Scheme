using System.Collections.Generic;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class Debtor
    {
        public string CreatedOn { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string ContactPreference { get; set; }
        public int ContactPreferenceId { get; set; }
        public string NotificationEmailAddress { get; set; }
        public CurrentAddress CurrentAddress { get; set; }
        public IEnumerable<PreviousAddress> PreviousAddresses { get; set; }
        public IEnumerable<Business> Businesses { get; set; }
        public IEnumerable<PreviousName> PreviousNames { get; set; }
    }
}