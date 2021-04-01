using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Gateways.Entities
{
    public class ntt_breathingspaceadhoccreditor : IDynamicsEntity
    {
        public Guid? ntt_breathingspaceadhoccreditorid { get; set; }
        public string ntt_name { get; set; }
        public List<Ntt_breathingspacecreditor> ntt_breathingspaceadhoccreditor_ntt_breathingspacecreditor_adhoccreditorid { get; set; }
        public List<Inss_InssAddress> ntt_breathingspaceadhoccreditor_inss_address { get; set; }

        public Guid GetId() => ntt_breathingspaceadhoccreditorid.Value;

        public IDictionary<string, object> ToAuditDictionary()
        {
            return new Dictionary<string, object>
            {
                { nameof(ntt_name), ntt_name },
            };
        }

        //createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //statecode:[Edm.Int32 Nullable=True]
        //_ownerid_value:[Edm.Guid Nullable=True]
        //_owningteam_value:[Edm.Guid Nullable=True]
        //_owninguser_value:[Edm.Guid Nullable=True]
        //timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //statuscode:[Edm.Int32 Nullable=True]
        //_modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //importsequencenumber:[Edm.Int32 Nullable=True]
        //_modifiedby_value:[Edm.Guid Nullable=True]
        //modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //_owningbusinessunit_value:[Edm.Guid Nullable=True]
        //_createdonbehalfby_value:[Edm.Guid Nullable=True]
        //ntt_createdonportal:[Edm.Boolean Nullable=True]
        //versionnumber:[Edm.Int64 Nullable=True]
        //_createdby_value:[Edm.Guid Nullable=True]
        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //ntt_breathingspaceadhoccreditor_ActivityPointers:[Collection([Microsoft.Dynamics.CRM.activitypointer Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_msfp_surveyinvites:[Collection([Microsoft.Dynamics.CRM.msfp_surveyinvite Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_msfp_surveyresponses:[Collection([Microsoft.Dynamics.CRM.msfp_surveyresponse Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_Appointments:[Collection([Microsoft.Dynamics.CRM.appointment Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_Emails:[Collection([Microsoft.Dynamics.CRM.email Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_Faxes:[Collection([Microsoft.Dynamics.CRM.fax Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_Letters:[Collection([Microsoft.Dynamics.CRM.letter Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_PhoneCalls:[Collection([Microsoft.Dynamics.CRM.phonecall Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_Tasks:[Collection([Microsoft.Dynamics.CRM.task Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_RecurringAppointmentMasters:[Collection([Microsoft.Dynamics.CRM.recurringappointmentmaster Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_SocialActivities:[Collection([Microsoft.Dynamics.CRM.socialactivity Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_ServiceAppointments:[Collection([Microsoft.Dynamics.CRM.serviceappointment Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_ntt_notificationemails:[Collection([Microsoft.Dynamics.CRM.ntt_notificationemail Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_ntt_notificationletters:[Collection([Microsoft.Dynamics.CRM.ntt_notificationletter Nullable=False]) Nullable=True]
        //ntt_breathingspaceadhoccreditor_ntt_notificationapis:[Collection([Microsoft.Dynamics.CRM.ntt_notificationapi Nullable=False]) Nullable=True]
        //
    }
}
