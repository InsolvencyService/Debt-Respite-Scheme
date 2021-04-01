using System;
using System.Collections.Generic;
using System.Linq;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Inss_cmpcreditor
    {
        public string inss_name { get; set; }
        public Guid? inss_cmpcreditorid { get; set; }
        public Guid? _inss_creditortypeid_value { get; set; }
        public List<Inss_creditordebttype> inss_cmpcreditor_inss_creditordebttype_CMPCreditorId { get; set; }
        public List<Inss_InssAddress> inss_cmpcreditor_inss_inssaddress_CMPCreditorI { get; set; }
        public List<Ntt_breathingspacecreditor> ntt_cmpcreditor_ntt_breathingspacecreditor_CMPCreditorId { get; set; }
        public DateTimeOffset? createdon { get; set; }
        public DateTimeOffset? modifiedon { get; set; }

        public List<Guid> GetDebtTypesIds()
        {
            if (inss_cmpcreditor_inss_creditordebttype_CMPCreditorId is null)
                return new List<Guid>();
            else
            {
                return inss_cmpcreditor_inss_creditordebttype_CMPCreditorId
                .Select(x => x._inss_debttypeid_value.Value)
                .ToList();
            }
        }
        //_modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //importsequencenumber:[Edm.Int32 Nullable=True]
        //ntt_telephonenumber:[Edm.String Nullable=True Unicode=False]
        //ntt_onboardeddate:[Edm.DateTimeOffset Nullable=True Precision=0]
        //_modifiedby_value:[Edm.Guid Nullable=True]
        //_owningteam_value:[Edm.Guid Nullable=True]
        //utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //statuscode:[Edm.Int32 Nullable=True]
        //statecode:[Edm.Int32 Nullable=True]
        //versionnumber:[Edm.Int64 Nullable=True]
        //createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //ntt_englandwalesregistered:[Edm.Boolean Nullable=True]
        //_owninguser_value:[Edm.Guid Nullable=True]
        //overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //ntt_companyregistrationnumber:[Edm.String Nullable=True Unicode=False]
        //_owningbusinessunit_value:[Edm.Guid Nullable=True]
        //_ownerid_value:[Edm.Guid Nullable=True]
        //modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //ntt_faxnumber:[Edm.String Nullable=True Unicode=False]
        //_createdby_value:[Edm.Guid Nullable=True]
        //_createdonbehalfby_value:[Edm.Guid Nullable=True]
        //timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //ntt_identifier:[Edm.String Nullable=True Unicode=False]        
        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //ntt_breathingspacecmpcreditor_ActivityPointers:[Collection([Microsoft.Dynamics.CRM.activitypointer Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_msfp_surveyinvites:[Collection([Microsoft.Dynamics.CRM.msfp_surveyinvite Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_msfp_surveyresponses:[Collection([Microsoft.Dynamics.CRM.msfp_surveyresponse Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_Appointments:[Collection([Microsoft.Dynamics.CRM.appointment Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_Emails:[Collection([Microsoft.Dynamics.CRM.email Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_Faxes:[Collection([Microsoft.Dynamics.CRM.fax Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_Letters:[Collection([Microsoft.Dynamics.CRM.letter Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_PhoneCalls:[Collection([Microsoft.Dynamics.CRM.phonecall Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_Tasks:[Collection([Microsoft.Dynamics.CRM.task Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_RecurringAppointmentMasters:[Collection([Microsoft.Dynamics.CRM.recurringappointmentmaster Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_SocialActivities:[Collection([Microsoft.Dynamics.CRM.socialactivity Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_ServiceAppointments:[Collection([Microsoft.Dynamics.CRM.serviceappointment Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_ntt_creditort:[Collection([Microsoft.Dynamics.CRM.ntt_creditortype Nullable=False]) Nullable=True]
        //ntt_CreditorTypeId:[Microsoft.Dynamics.CRM.ntt_creditortype Nullable=False]
        //ntt_breathingspacecmpcreditor_ntt_notificationletters:[Collection([Microsoft.Dynamics.CRM.ntt_notificationletter Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_ntt_notificationemails:[Collection([Microsoft.Dynamics.CRM.ntt_notificationemail Nullable=False]) Nullable=True]
        //ntt_breathingspacecmpcreditor_ntt_creditorbusinesscontact_CreditorOrganisationId:[Collection([Microsoft.Dynamics.CRM.ntt_creditorbusinesscontact Nullable=False]) Nullable=True]
    }
}