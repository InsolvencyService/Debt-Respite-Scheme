using System;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Ntt_contactrole
    {
        public Guid? ntt_contactroleid { get; set; }
        public int ntt_subrole { get; set; }
        public Ntt_breathingspacemoratorium ntt_BreathingSpaceMoratoriumid { get; set; }
        public Ntt_externalcontact ntt_ExternalContactid { get; set; }
        public DateTimeOffset? createdon { get; set; }

        public PointContactRoleType GetMappedPointContactRoleId(DynamicsGatewayOptions options)
        {
            _ = options.PointContactRoleRead.TryGetValue(ntt_subrole.ToString(), out var roleId);

            return (PointContactRoleType)roleId;
        }

        //        _owningteam_value:[Edm.Guid Nullable=True]
        //        _owninguser_value:[Edm.Guid Nullable=True]
        //        _modifiedby_value:[Edm.Guid Nullable=True]
        //        _owningbusinessunit_value:[Edm.Guid Nullable=True]
        //        versionnumber:[Edm.Int64 Nullable=True]
        //        _ntt_breathingspacemoratoriumid_value:[Edm.Guid Nullable=True]
        //        importsequencenumber:[Edm.Int32 Nullable=True]
        //        _modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //        utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //        statuscode:[Edm.Int32 Nullable=True]
        //        statecode:[Edm.Int32 Nullable=True]
        //        overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //public int? ntt_contactrole { get; set; }
        //        _ownerid_value:[Edm.Guid Nullable=True]
        //public string ntt_name { get; set; }
        //        _createdby_value:[Edm.Guid Nullable=True]
        //        modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //        _ntt_externalcontactid_value:[Edm.Guid Nullable=True]
        //        _createdonbehalfby_value:[Edm.Guid Nullable=True]
        //        createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //        owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //        ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //        owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //        ntt_contactrole_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_contactrole_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_contactrole_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_contactrole_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_contactrole_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_contactrole_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //public Ntt_externalcontact ntt_contactrole_ntt_externalcontact_ContactRoleId { get; set; }
    }
}
