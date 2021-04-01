using System;
using System.Linq;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Responses;

namespace Insolvency.Integration.Gateways.Entities
{
    public class ntt_debteligibilityreview
    {
        public Ntt_breathingspacedebt ntt_DebtId { get; set; }
        public Guid? ntt_debteligibilityreviewid { get; set; }
        public Guid? _ntt_debtid_value { get; set; }
        public Guid? _ntt_eligibilitystatusid_value { get; set; }
        public string ntt_creditornotes { get; set; }
        public string ntt_moneyadvisernotes { get; set; }
        public Guid? _ntt_debteligibilityreviewtypeid_value { get; set; }
        public DateTimeOffset? createdon { get; set; }

        public DebtEligibilityReviewStatus GetEligiblityReviewStatus(DynamicsGatewayOptions options) =>
            options.DebtEligibilityReviewStatus[_ntt_eligibilitystatusid_value.ToString()];

        public DebtEligibilityReviewReasons GetEligiblityReviewReason(DynamicsGatewayOptions options)
        {
            var reviewStatus = options.DebtEligibilityReviewReasons
                .First(r => r.Value == _ntt_debteligibilityreviewtypeid_value.ToString()).Key;

            return (DebtEligibilityReviewReasons)int.Parse(reviewStatus);
        }

        public DebtEligibilityReviewResponse ToDebtEligibilityReview(DynamicsGatewayOptions options)
        {
            return new DebtEligibilityReviewResponse
            {
                //DebtReviewId = ntt_debteligibilityreviewid.Value,
                CreditorNotes = ntt_creditornotes,
                Reason = GetEligiblityReviewReason(options),
                Status = GetEligiblityReviewStatus(options),
                MoneyAdviserNotes = ntt_moneyadvisernotes,
            };
        }

        //        ntt_statuslastupdateddate:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        ntt_courtdecisionnotes:[Edm.String Nullable=True Unicode=False]
        //        _ntt_eligibilitystatusid_value:[Edm.Guid Nullable=True]
        //        _ownerid_value:[Edm.Guid Nullable=True]
        //        versionnumber:[Edm.Int64 Nullable=True]
        //        statuscode:[Edm.Int32 Nullable=True]
        //        utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //        createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        importsequencenumber:[Edm.Int32 Nullable=True]
        //        _owningbusinessunit_value:[Edm.Guid Nullable=True]
        //        _modifiedby_value:[Edm.Guid Nullable=True]
        //        modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        statecode:[Edm.Int32 Nullable=True]
        //        _owningteam_value:[Edm.Guid Nullable=True]
        //        ntt_name:[Edm.String Nullable=True Unicode=False]
        //        ntt_moneyadvisernotes:[Edm.String Nullable=True Unicode=False]
        //        _modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //        overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _createdonbehalfby_value:[Edm.Guid Nullable=True]
        //        _owninguser_value:[Edm.Guid Nullable=True]
        //        _createdby_value:[Edm.Guid Nullable=True]
        //        timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //        createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //        ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //        owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //        ntt_debteligibilityreview_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_DuplicateMatchingRecord:[Collection([Microsoft.Dynamics.CRM.duplicaterecord Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_DuplicateBaseRecord:[Collection([Microsoft.Dynamics.CRM.duplicaterecord Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ActivityPointers:[Collection([Microsoft.Dynamics.CRM.activitypointer Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_msfp_surveyinvites:[Collection([Microsoft.Dynamics.CRM.msfp_surveyinvite Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_msfp_surveyresponses:[Collection([Microsoft.Dynamics.CRM.msfp_surveyresponse Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ntt_notificationemails:[Collection([Microsoft.Dynamics.CRM.ntt_notificationemail Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ntt_notificationletters:[Collection([Microsoft.Dynamics.CRM.ntt_notificationletter Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ntt_notificationapis:[Collection([Microsoft.Dynamics.CRM.ntt_notificationapi Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_msfp_alerts:[Collection([Microsoft.Dynamics.CRM.msfp_alert Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_Appointments:[Collection([Microsoft.Dynamics.CRM.appointment Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_Emails:[Collection([Microsoft.Dynamics.CRM.email Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_Faxes:[Collection([Microsoft.Dynamics.CRM.fax Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_Letters:[Collection([Microsoft.Dynamics.CRM.letter Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_PhoneCalls:[Collection([Microsoft.Dynamics.CRM.phonecall Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_Tasks:[Collection([Microsoft.Dynamics.CRM.task Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_RecurringAppointmentMasters:[Collection([Microsoft.Dynamics.CRM.recurringappointmentmaster Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_SocialActivities:[Collection([Microsoft.Dynamics.CRM.socialactivity Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ServiceAppointments:[Collection([Microsoft.Dynamics.CRM.serviceappointment Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //        ntt_EligibilityStatusId:[Microsoft.Dynamics.CRM.ntt_debteligibilitystatustype Nullable=False]
        //        ntt_DebtEligibilityReviewTypeId:[Microsoft.Dynamics.CRM.ntt_debteligibilityreviewtype Nullable=False]
        //        ntt_debteligibilityreview_ntt_moneyadviserorganisationtasks:[Collection([Microsoft.Dynamics.CRM.ntt_moneyadviserorganisationtask Nullable=False]) Nullable=True]
        //ntt_debteligibilityreview_ntt_creditororganisationtasks:[Collection([Microsoft.Dynamics.CRM.ntt_creditororganisationtask Nullable=False]) Nullable=True]

    }
}