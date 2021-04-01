using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Gateways.Entities
{
    public class ntt_previousname : IDynamicsEntity
    {
        public virtual string ntt_firstname { get; set; }
        public virtual string ntt_middlename { get; set; }
        public virtual string ntt_lastname { get; set; }
        public virtual Guid? ntt_previousnameid { get; set; }
        public virtual DateTimeOffset? createdon { get; set; }
        public virtual Ntt_breathingspacedebtor ntt_DebtorId { get; set; }
        public Guid GetId() => ntt_previousnameid.Value;

        //        statuscode:[Edm.Int32 Nullable=True]
        //        overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _owninguser_value:[Edm.Guid Nullable=True]
        //        _modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //        timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //        ntt_name:[Edm.String Nullable=True Unicode=False]
        //        versionnumber:[Edm.Int64 Nullable=True]
        //        _owningteam_value:[Edm.Guid Nullable=True]
        //        createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _createdonbehalfby_value:[Edm.Guid Nullable=True]
        //        ntt_modifiedbyemailaddress:[Edm.String Nullable=True Unicode=False]
        //        _modifiedby_value:[Edm.Guid Nullable=True]
        //        _owningbusinessunit_value:[Edm.Guid Nullable=True]
        //        utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //        importsequencenumber:[Edm.Int32 Nullable=True]
        //        modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _ownerid_value:[Edm.Guid Nullable=True]
        //        _ntt_debtorid_value:[Edm.Guid Nullable=True]
        //        statecode:[Edm.Int32 Nullable=True]
        //        _createdby_value:[Edm.Guid Nullable=True]
        //        ntt_modifiedbyname:[Edm.String Nullable=True Unicode=False]
        //        createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //        ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //        owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //        ntt_previousname_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_previousname_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_previousname_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_previousname_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_previousname_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_previousname_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_DebtorId:[Microsoft.Dynamics.CRM.ntt_breathingspacedebtor Nullable=False]

        public IDictionary<string, object> ToAuditDictionary()
        {
            return new Dictionary<string, object>
            {
                { nameof(ntt_firstname), ntt_firstname},
                { nameof(ntt_middlename), ntt_middlename},
                { nameof(ntt_lastname), ntt_lastname}
            };
        }
    }
}
