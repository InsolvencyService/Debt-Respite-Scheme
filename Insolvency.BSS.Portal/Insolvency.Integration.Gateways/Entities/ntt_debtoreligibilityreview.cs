using System;
using System.Collections.Generic;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.Entities
{
    public class ntt_debtoreligibilityreview
    {
        public Guid ntt_debtoreligibilityreviewid { get; set; }
        public Guid _ntt_moratoriumid_value { get; set; }
        public Guid _ntt_creditorid_value { get; set; }
        public Guid? _ntt_debtoreligibilityreviewchildstatusid_value { get; set; }
        public Guid? _ntt_eligibilitystatusid_value { get; set; }
        public Guid? _modifiedonbehalfby_value { get; set; }
        public Guid? _ownerid_value { get; set; }
        public Guid? _owningteam_value { get; set; }
        public Guid? _modifiedby_value { get; set; }
        public Guid? _ntt_debtoreligibilityreviewstatusid_value { get; set; }
        public Guid? _createdonbehalfby_value { get; set; }
        public Guid? _owningbusinessunit_value { get; set; }
        public Guid? _createdby_value { get; set; }
        public Guid? _owninguser_value { get; set; }
        public string ntt_creditornotes { get; set; }
        public string ntt_courtdecisionnotes { get; set; }
        public string ntt_moneyadvisernotes { get; set; }
        public string ntt_name { get; set; }
        public DateTimeOffset? modifiedon { get; set; }
        public DateTimeOffset? ntt_statuslastupdateddate { get; set; }
        public DateTimeOffset? createdon { get; set; }
        public DateTimeOffset? overriddencreatedon { get; set; }
        public long? versionnumber { get; set; }
        public int? statuscode { get; set; }
        public int? statecode { get; set; }
        public int? utcconversiontimezonecode { get; set; }
        public int? importsequencenumber { get; set; }
        public int? timezoneruleversionnumber { get; set; }
        public Ntt_breathingspacecreditor ntt_CreditorId { get; set; }
        public Ntt_breathingspacemoratorium ntt_MoratoriumId { get; set; }
        public List<ntt_moneyadviserorganisationtask> ntt_debtoreligibilityreview_ntt_moneyadviserorganisationtasks { get; set; }

        public DebtorEligibilityReviewResponse ToDebtorEligibilityReview(DynamicsGatewayOptions options) => new DebtorEligibilityReviewResponse
        {
            CreditorId = _ntt_creditorid_value,
            CreditorNotes = ntt_creditornotes,
            CreditorName = ntt_CreditorId?.ntt_name,
            Status = options.DebtorEligibilityReviewStatus[_ntt_eligibilitystatusid_value.ToString()],
            CreatedOn = createdon.Value,
            EndReason = options.GetEligibilityReviewParentReasonById(_ntt_debtoreligibilityreviewstatusid_value),
            NoLongerEligibleReason = options.GetEligibilityReviewChildReasonById(_ntt_debtoreligibilityreviewchildstatusid_value),
        };


        //versionnumber:[Edm.Int64 Nullable=True]
        //_ntt_debtoreligibilityreviewchildstatusid_value:[Edm.Guid Nullable=True]
        //statuscode:[Edm.Int32 Nullable=True]
        //_ntt_eligibilitystatusid_value:[Edm.Guid Nullable=True]
        //modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //ntt_statuslastupdateddate:[Edm.DateTimeOffset Nullable=True Precision=0]
        //_modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //_ownerid_value:[Edm.Guid Nullable=True]
        //_owningteam_value:[Edm.Guid Nullable=True]
        //statecode:[Edm.Int32 Nullable=True]
        //utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //_modifiedby_value:[Edm.Guid Nullable=True]
        //_ntt_debtoreligibilityreviewstatusid_value:[Edm.Guid Nullable=True]
        //_createdonbehalfby_value:[Edm.Guid Nullable=True]
        //importsequencenumber:[Edm.Int32 Nullable=True]
        //_owningbusinessunit_value:[Edm.Guid Nullable=True]
        //ntt_courtdecisionnotes:[Edm.String Nullable=True Unicode=False]
        //_createdby_value:[Edm.Guid Nullable=True]
        //overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //ntt_creditornotes:[Edm.String Nullable=True Unicode=False]
        //_owninguser_value:[Edm.Guid Nullable=True]
        //ntt_moneyadvisernotes:[Edm.String Nullable=True Unicode=False]
        //ntt_name:[Edm.String Nullable=True Unicode=False]
        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //ntt_debtoreligibilityreview_ActivityPointers:[Collection([Microsoft.Dynamics.CRM.activitypointer Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_msfp_surveyinvites:[Collection([Microsoft.Dynamics.CRM.msfp_surveyinvite Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_msfp_surveyresponses:[Collection([Microsoft.Dynamics.CRM.msfp_surveyresponse Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_ntt_notificationemails:[Collection([Microsoft.Dynamics.CRM.ntt_notificationemail Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_ntt_notificationletters:[Collection([Microsoft.Dynamics.CRM.ntt_notificationletter Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_ntt_notificationapis:[Collection([Microsoft.Dynamics.CRM.ntt_notificationapi Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_msfp_alerts:[Collection([Microsoft.Dynamics.CRM.msfp_alert Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_Appointments:[Collection([Microsoft.Dynamics.CRM.appointment Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_Emails:[Collection([Microsoft.Dynamics.CRM.email Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_Faxes:[Collection([Microsoft.Dynamics.CRM.fax Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_Letters:[Collection([Microsoft.Dynamics.CRM.letter Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_PhoneCalls:[Collection([Microsoft.Dynamics.CRM.phonecall Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_Tasks:[Collection([Microsoft.Dynamics.CRM.task Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_RecurringAppointmentMasters:[Collection([Microsoft.Dynamics.CRM.recurringappointmentmaster Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_SocialActivities:[Collection([Microsoft.Dynamics.CRM.socialactivity Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //ntt_debtoreligibilityreview_ServiceAppointments:[Collection([Microsoft.Dynamics.CRM.serviceappointment Nullable=False]) Nullable=True]
        //ntt_CreditorId:[Microsoft.Dynamics.CRM.ntt_breathingspacecreditor Nullable=False]
        //ntt_EligibilityStatusId:[Microsoft.Dynamics.CRM.ntt_debtoreligibilitystatustype Nullable=False]
        //ntt_DebtorEligibilityReviewStatusId:[Microsoft.Dynamics.CRM.ntt_debtoreligibilityreviewstatustype Nullable=False]
        //ntt_debtoreligibilityreview_ntt_moneyadviserorganisationtasks:[Collection([Microsoft.Dynamics.CRM.ntt_moneyadviserorganisationtask Nullable=False]) Nullable=True]
        //ntt_DebtorEligibilityReviewChildStatusId:[Microsoft.Dynamics.CRM.ntt_debtoreligibilityreviewchildstatustype Nullable=False]
    }
}