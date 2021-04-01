using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.Shared.Responses

{
    public class DebtorDetailsResponse
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<ClientPreviousNameResponse> PreviousNames { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public bool? IsInMentalHealthMoratorium { get; set; }
        public string ReferenceNumber { get; set; }
        public bool AddressHidden { get; set; }
        public ContactPreferenceType? ContactPreference { get; set; }
        public string CancellationReason { get; set; }

        [JsonIgnore]
        public string ContactPreferenceLabel => ContactPreference
            switch
        {
            ContactPreferenceType.None => "Client does not want to receive notifications",
            ContactPreferenceType.Email => "Email",
            ContactPreferenceType.Post => "Post",
            _ => ""
        };

        public string NotificationEmailAddress { get; set; }
        public DateTimeOffset? StartsOn { get; set; }
        public DateTimeOffset? EndsOn { get; set; }
        public DateTimeOffset? CancellationDate { get; set; }
        public MoratoriumStatus MoratoriumStatus { get; set; }
        public DateTimeOffset? CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
