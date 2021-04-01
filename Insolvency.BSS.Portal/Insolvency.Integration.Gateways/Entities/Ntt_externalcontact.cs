using System;
using System.Collections.Generic;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Ntt_externalcontact
    {
        public Guid? ntt_externalcontactid { get; set; }
        public string ntt_firstname { get; set; }
        public string ntt_lastname { get; set; }
        public string ntt_emailaddress { get; set; }
        public string ntt_hometelephonenumber { get; set; }
        public int ntt_preferredchannel { get; set; }
        public List<Inss_InssAddress> ntt_externalcontact_inss_inssaddress { get; set; }

        public List<Ntt_contactrole> ntt_externalcontact_ntt_contactrole_ExternalContact { get; set; }

        public PointContactConfirmationMethod GetMappedPreferredChannelId(DynamicsGatewayOptions options)
        {
            _ = options.ContactPreferenceReadMap.TryGetValue(ntt_preferredchannel.ToString(), out var channelId);

            return (PointContactConfirmationMethod)channelId;
        }

        //        importsequencenumber:[Edm.Int32 Nullable=True]
        //        ntt_email:[Edm.Boolean Nullable=True]
        //        utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //        ntt_middlename:[Edm.String Nullable=True Unicode=False]
        //        _createdonbehalfby_value:[Edm.Guid Nullable=True]
        //        _ownerid_value:[Edm.Guid Nullable=True]
        //        ntt_lastname:[Edm.String Nullable=True Unicode=False]
        //        _owningbusinessunit_value:[Edm.Guid Nullable=True]
        //        ntt_mobiletelephonenumber:[Edm.String Nullable=True Unicode=False]
        //        overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        ntt_telephone:[Edm.Boolean Nullable=True]
        //        ntt_firstname:[Edm.String Nullable=True Unicode=False]
        //        versionnumber:[Edm.Int64 Nullable=True]
        //        ntt_mobiletelephone:[Edm.Boolean Nullable=True]
        //        statuscode:[Edm.Int32 Nullable=True]
        //        _owningteam_value:[Edm.Guid Nullable=True]
        //        statecode:[Edm.Int32 Nullable=True]
        //        ntt_dateofbirth:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _owninguser_value:[Edm.Guid Nullable=True]
        //        timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //        createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //        _ntt_contactroleid_value:[Edm.Guid Nullable=True]
        //        _modifiedby_value:[Edm.Guid Nullable=True]
        //        _createdby_value:[Edm.Guid Nullable=True]
        //        createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //        ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //        owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //        ntt_externalcontact_ActivityPointers:[Collection([Microsoft.Dynamics.CRM.activitypointer Nullable=False]) Nullable=True]
        //ntt_externalcontact_msfp_surveyinvites:[Collection([Microsoft.Dynamics.CRM.msfp_surveyinvite Nullable=False]) Nullable=True]
        //ntt_externalcontact_msfp_surveyresponses:[Collection([Microsoft.Dynamics.CRM.msfp_surveyresponse Nullable=False]) Nullable=True]
        //ntt_externalcontact_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_externalcontact_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_externalcontact_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_externalcontact_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_externalcontact_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_externalcontact_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_externalcontact_Appointments:[Collection([Microsoft.Dynamics.CRM.appointment Nullable=False]) Nullable=True]
        //ntt_externalcontact_Emails:[Collection([Microsoft.Dynamics.CRM.email Nullable=False]) Nullable=True]
        //ntt_externalcontact_Faxes:[Collection([Microsoft.Dynamics.CRM.fax Nullable=False]) Nullable=True]
        //ntt_externalcontact_Letters:[Collection([Microsoft.Dynamics.CRM.letter Nullable=False]) Nullable=True]
        //ntt_externalcontact_PhoneCalls:[Collection([Microsoft.Dynamics.CRM.phonecall Nullable=False]) Nullable=True]
        //ntt_externalcontact_Tasks:[Collection([Microsoft.Dynamics.CRM.task Nullable=False]) Nullable=True]
        //ntt_externalcontact_RecurringAppointmentMasters:[Collection([Microsoft.Dynamics.CRM.recurringappointmentmaster Nullable=False]) Nullable=True]
        //ntt_externalcontact_SocialActivities:[Collection([Microsoft.Dynamics.CRM.socialactivity Nullable=False]) Nullable=True]
        //ntt_externalcontact_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //ntt_externalcontact_ServiceAppointments:[Collection([Microsoft.Dynamics.CRM.serviceappointment Nullable=False]) Nullable=True]
        //ntt_externalcontact_ntt_notificationemails:[Collection([Microsoft.Dynamics.CRM.ntt_notificationemail Nullable=False]) Nullable=True]
        //ntt_externalcontact_ntt_notificationletters:[Collection([Microsoft.Dynamics.CRM.ntt_notificationletter Nullable=False]) Nullable=True]
        //ntt_externalcontact_ntt_contactrole_ExternalContact:[Collection([Microsoft.Dynamics.CRM.ntt_contactrole Nullable=False]) Nullable=True]
        //ntt_externalcontact_ntt_notificationapis:[Collection([Microsoft.Dynamics.CRM.ntt_notificationapi Nullable=False]) Nullable=True]
        //public Ntt_contactrole ntt_ContactRoleId { get; set; }
        //        ntt_externalcontact_msfp_alerts:[Collection([Microsoft.Dynamics.CRM.msfp_alert Nullable=False]) Nullable=True]

    }
}
