using System;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Models.MoneyAdviserService.Responses
{
    public class AccountSearchResponse
    {
        public Guid BreathingSpaceId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string BreathingSpaceReference { get; set; }
        public DateTime DateOfBirth { get; set; }
        public AddressResponse Address { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MoratoriumStatus MoratoriumStatus { get; set; }
        public string  OrganisationName { get; set; }
        public string MoratoriumType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
