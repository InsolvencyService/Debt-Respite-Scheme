using System.Collections.Generic;

namespace Insolvency.Integration.Gateways.Audit
{
    public static class AuditContextDictionaryExtensions
    {
        public static IDictionary<string, object> SetDynamicsActionAuditParameters(this IDictionary<string, object> dict, AuditContext audit)
        {
            if (dict.ContainsKey("ModifiedByClientId"))
            {
                return dict;
            }
            dict.Add("ModifiedByEmailAddress", audit?.Email);
            dict.Add("ModifiedByName", audit?.Name);
            dict.Add("ModifiedByCreditorOrganisation", audit?.CreditorOrganisationId);
            dict.Add("ModifiedByMoneyAdviserOrganisation", audit?.MoneyAdviserOrganisationId);
            dict.Add("ModifiedByClientId", audit?.ClientId);
            return dict;
        }

        public static IDictionary<string, object> SetDynamicsObjectAuditProperties(this IDictionary<string, object> dict, AuditContext audit)
        {
            if (dict.ContainsKey("ntt_modifiedbyclientid"))
            {
                return dict;
            }
            dict.Add("ntt_modifiedbyemailaddress", audit?.Email);
            dict.Add("ntt_modifiedbyname", audit?.Name);
            dict.Add("ntt_modifiedbycreditororganisation", audit?.CreditorOrganisationId);
            dict.Add("ntt_modifiedbymoneyadviserorganisation", audit?.MoneyAdviserOrganisationId);
            dict.Add("ntt_modifiedbyclientid", audit?.ClientId);
            return dict;
        }
    }
}
