using System;
using Insolvency.Integration.Models;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Ntt_breathingspacecreditor : IDynamicsEntity
    {
        public Guid? _owninguser_value { get; set; }
        public Guid? _ownerid_value { get; set; }
        public int? utcconversiontimezonecode { get; set; }
        public Guid? _createdby_value { get; set; }
        public DateTimeOffset? createdon { get; set; }
        public string ntt_name { get; set; }
        public Guid? ntt_breathingspacecreditorid { get; set; }
        public Guid? _ntt_adhoccreditorid_value { get; set; }
        public Guid? _createdonbehalfby_value { get; set; }
        public string ntt_notificationemailaddress { get; set; }
        public DateTimeOffset? modifiedon { get; set; }
        public Guid? _ntt_cmpcreditorid_value { get; set; }
        public Guid? _owningteam_value { get; set; }
        public Guid? _owningbusinessunit_value { get; set; }
        public int? importsequencenumber { get; set; }
        public long? versionnumber { get; set; }
        public Guid? _modifiedonbehalfby_value { get; set; }
        public Guid? _modifiedby_value { get; set; }
        public int? timezoneruleversionnumber { get; set; }
        public DateTimeOffset? overriddencreatedon { get; set; }
        public int? statecode { get; set; }
        public Guid? _ntt_notificationaddressid_value { get; set; }
        public int? statuscode { get; set; }
        public Inss_cmpcreditor ntt_CMPCreditorId { get; set; }
        public ntt_breathingspaceadhoccreditor ntt_adhoccreditorid { get; set; }
        public Inss_InssAddress ntt_NotificationAddressId { get; set; }

        public Guid GetId() => ntt_breathingspacecreditorid.Value;

        public Creditor ToCreditorModel()
        {
            return new Creditor
            {
                Name = ntt_name,
                Id = ntt_breathingspacecreditorid.Value
            };
        }

        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //ntt_breathingspacecreditor_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_breathingspacecreditor_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_breathingspacecreditor_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_breathingspacecreditor_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_breathingspacecreditor_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_breathingspacecreditor_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_breathingspacecreditor_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //ntt_CMPCreditor1:[Microsoft.Dynamics.CRM.ntt_breathingspacecmpcreditor Nullable=False]
        //ntt_breathingspacecreditor_ntt_breathingspacedebt_CreditorId:[Collection([Microsoft.Dynamics.CRM.ntt_breathingspacedebt Nullable=False]) Nullable=True]

    }
}
