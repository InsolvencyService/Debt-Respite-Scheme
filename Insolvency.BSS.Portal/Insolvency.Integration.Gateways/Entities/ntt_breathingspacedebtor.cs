using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Ntt_breathingspacedebtor : IDynamicsEntity
    {
        public virtual string ntt_firstname { get; set; }
        public virtual long? Versionnumber { get; set; }
        public virtual int? Utcconversiontimezonecode { get; set; }
        public virtual bool? Ntt_mobiletelephone { get; set; }
        public virtual string ntt_lastname { get; set; }
        public virtual bool? Ntt_addresswithheld { get; set; }
        public virtual Guid? Ntt_breathingspacedebtorid { get; set; }
        public virtual string Ntt_hometelephonenumber { get; set; }
        public virtual int? Ntt_preferredchannel { get; set; }
        public virtual Guid? _ownerid_value { get; set; }
        public virtual string Ntt_mobiletelephonenumber { get; set; }
        public virtual Guid? _owningbusinessunit_value { get; set; }
        public virtual int? Timezoneruleversionnumber { get; set; }
        public virtual DateTimeOffset? Modifiedon { get; set; }
        public virtual bool? Ntt_telephone { get; set; }
        public virtual Guid? _owningteam_value { get; set; }
        public virtual string Ntt_name { get; set; }
        public virtual Guid? _modifiedby_value { get; set; }
        public virtual Guid? _createdby_value { get; set; }
        public virtual string ntt_middlename { get; set; }
        public virtual DateTimeOffset? Overriddencreatedon { get; set; }
        public virtual DateTimeOffset? ntt_dateofbirth { get; set; }
        public virtual bool? Ntt_email { get; set; }
        public virtual string Ntt_emailaddress { get; set; }
        public virtual Guid? _modifiedonbehalfby_value { get; set; }
        public virtual Guid? _owninguser_value { get; set; }
        public virtual int? Statecode { get; set; }
        public virtual DateTimeOffset? Createdon { get; set; }
        public virtual Guid? _createdonbehalfby_value { get; set; }
        public virtual int? Importsequencenumber { get; set; }
        public virtual int? Statuscode { get; set; }
        public virtual List<Inss_InssAddress> ntt_breathingspacedebtor_inss_inssaddress { get; set; }
        public virtual List<Ntt_breathingspacemoratorium> Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid { get; set; }
        public List<Inss_business> ntt_breathingspacedebtor_inss_business_DebtorId { get; set; }
        public virtual List<ntt_previousname> ntt_breathingspacedebtor_ntt_previousname_DebtorId { get; set; }

        //public virtual global::Microsoft.Dynamics.CRM.Ntt_breathingspacedebtorstatustype Ntt_statusid { get; set; }

        public ContactPreferenceType GetMappedPreferredChannelId(DynamicsGatewayOptions options)
        {
            if (options.ContactPreferenceReadMap.TryGetValue(Ntt_preferredchannel.ToString(), out var preference))
                return (ContactPreferenceType)preference;
            else return ContactPreferenceType.None;
        }

        public Guid GetId() => Ntt_breathingspacedebtorid.Value;

        public Inss_InssAddress GetCurrentAddress()
        {
            return ntt_breathingspacedebtor_inss_inssaddress
                ?.FirstOrDefault(add => add.inss_dateto is null || add.inss_dateto == default);
        }

        public List<Guid> GetMoratoriumIds() => 
            Ntt_breathingspacedebtor_ntt_breathingspacemoratorium_debtorid
                ?.Select(x => x.GetId())
                .ToList() ?? new List<Guid>();

        public IDictionary<string, object> ToAuditDictionary()
        {
            return new Dictionary<string, object>
            {
                { nameof(ntt_firstname), ntt_firstname },
                { nameof(ntt_middlename), ntt_middlename },
                { nameof(ntt_lastname), ntt_lastname },
                { nameof(ntt_dateofbirth), ntt_dateofbirth }
            };
        }
    }
}
