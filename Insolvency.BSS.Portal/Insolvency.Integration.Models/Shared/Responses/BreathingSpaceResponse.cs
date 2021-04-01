using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Models.Shared.Responses
{
    public class BreathingSpaceResponse
    {
        [JsonIgnore]
        public Guid OrganisationId { get; set; }
        
        public MoneyAdviserOrganizationResponse MoneyAdviserOrganization { get; set; }
        public DebtorDetailsResponse DebtorDetails { get; set; }
        public AddressResponse CurrentAddress { get; set; }
        public IEnumerable<PreviousAddressResponse> PreviousAddresses { get; set; }
        public IEnumerable<BusinessAddressResponse> DebtorBusinessDetails { get; set; }
        public IEnumerable<DebtDetailResponse> DebtDetails { get; set; } = new List<DebtDetailResponse>();
        public IEnumerable<DebtorEligibilityReviewResponse> DebtorEligibilityReviews { get; set; }
        public DebtorNominatedContactResponse DebtorNominatedContactResponse { get; set; }
        public DebtorTransferResponse DebtorTransfer { get; set; }
    }
}
