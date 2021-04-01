using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class CreateBreathingSpace
    {
        [Required]
        public BSCreationDebtorContactPreference DebtorContactPreference { get; set; }
        [Required]
        public BSCreationClientDetailsCreate DebtorDetails { get; set; }
        [Required]
        public BSCreationAddress CurrentAddress { get; set; }

        public List<BSCreationPreviousAddress> PreviousAddresses { get; set; }
        public List<BSCreationDebt> Debts { get; set; }
        public List<BSCreationAdHocDebt> AdHocDebts { get; set; }
        public List<BSCreationBusiness> Businesses { get; set; }
        public List<BSCreationDebtorNominatedContact> DebtorNominatedContacts { get; set; }

        public virtual Dictionary<string, object> ToDictionary(DynamicsGatewayOptions options, Guid organisationId, IDictionary<string,object> auditOrigin)
        {
            var dictionary = new Dictionary<string, object>
            {
                { "ContactPreference", DebtorContactPreference.ToDictionary() },
                { "DebtorDetails", DebtorDetails.ToDictionary() },
                { "CurrentAddress", CurrentAddress.ToDictionary() },
                { "PreviousAddresses", PreviousAddresses?.Select(x => x.ToDictionary()).ToList() },
                { "Debts", Debts?.Select(x => x.ToDictionary()).ToList() },
                { "AdHocDebts", AdHocDebts?.Select(x => x.ToDictionary()).ToList() },
                { "Businesses", Businesses?.Select(x => x.ToDictionary()).ToList() },
                { "DebtorNominatedContacts", DebtorNominatedContacts?.Select(x => x.ToDictionary(options)).ToList() },
                { "ManagingMoneyAdviserOrganisationId", organisationId },
                { "Origin", auditOrigin }
            };

            var payload = JsonSerializer.Serialize(new Dictionary<string, object>
                    {
                        { "BreathingSpaceMoratorium",  dictionary}
                    });

            var dictionaryWrapper = new Dictionary<string, object>
            {
                { "Request", payload }
            };

            return dictionaryWrapper;
        }
    }
}
