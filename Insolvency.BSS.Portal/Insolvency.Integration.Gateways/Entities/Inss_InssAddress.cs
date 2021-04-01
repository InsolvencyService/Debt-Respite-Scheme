using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Inss_InssAddress : IDynamicsEntity
    {
        public virtual string inss_addressline1 { get; set; }
        public virtual Guid? _owningteam_value { get; set; }
        public virtual int? Importsequencenumber { get; set; }
        public virtual int? Utcconversiontimezonecode { get; set; }
        public virtual string inss_addressline5 { get; set; }
        public virtual Guid? _owningbusinessunit_value { get; set; }
        public virtual string inss_addressline2 { get; set; }
        public virtual string inss_postcode { get; set; }
        public virtual DateTimeOffset? Createdon { get; set; }
        public virtual string inss_addressline3 { get; set; }
        public virtual string inss_name { get; set; }
        public virtual DateTimeOffset? inss_dateto { get; set; }
        public virtual int? Statecode { get; set; }
        public virtual DateTimeOffset? Modifiedon { get; set; }
        public virtual Guid? _createdby_value { get; set; }
        public virtual int? inss_addresstype { get; set; }
        public virtual Guid? _ownerid_value { get; set; }
        public virtual Guid? _owninguser_value { get; set; }
        public virtual string inss_country { get; set; }
        public virtual DateTimeOffset? inss_datefrom { get; set; }
        public virtual int? Statuscode { get; set; }
        public virtual Guid? _modifiedby_value { get; set; }
        public virtual Guid? inss_inssaddressid { get; set; }
        public virtual DateTimeOffset? Overriddencreatedon { get; set; }
        public virtual Guid? _createdonbehalfby_value { get; set; }
        public virtual long? Versionnumber { get; set; }
        public virtual string inss_uprn { get; set; }
        public virtual Guid? _modifiedonbehalfby_value { get; set; }
        public virtual string inss_addressline4 { get; set; }
        public virtual int? Timezoneruleversionnumber { get; set; }
        public virtual List<Ntt_breathingspacedebtor> ntt_breathingspacedebtor_inss_inssaddress { get; set; }

        public Guid GetId() => inss_inssaddressid.Value;

        public IDictionary<string, object> ToAuditDictionary()
        {
            return new Dictionary<string, object>
            {
                { nameof(inss_addressline1), inss_addressline1 },
                { nameof(inss_addressline2), inss_addressline2 },
                { nameof(inss_postcode), inss_postcode },
                { nameof(inss_addressline4), inss_addressline4 },
                { nameof(inss_addressline3), inss_addressline3 },
                { nameof(inss_country), inss_country },
                { nameof(inss_datefrom), inss_datefrom },
                { nameof(inss_dateto), inss_dateto }
            };
        }
    }
}
