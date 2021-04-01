using System;
using Insolvency.Integration.Models;

namespace Insolvency.Integration.Gateways.Entities
{
    public class INSS_debttype
    {
        public string inss_name { get; set; }
        public Guid inss_debttypeid { get; set; }
        public string inss_identifier { get; set; }

        public DebtType MapTopDebtType() =>
            new DebtType { Id = inss_debttypeid, Name = inss_name };

        //utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //statecode:[Edm.Int32 Nullable=True]
        //_createdby_value:[Edm.Guid Nullable=True]
        //modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //statuscode:[Edm.Int32 Nullable=True]
        //importsequencenumber:[Edm.Int32 Nullable=True]
        //_modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //_modifiedby_value:[Edm.Guid Nullable=True]
        //createdon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //_createdonbehalfby_value:[Edm.Guid Nullable=True]
        //timezoneruleversionnumber:[Edm.Int32 Nullable=True]
        //_organizationid_value:[Edm.Guid Nullable=True]
        //versionnumber:[Edm.Int64 Nullable=True]
        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //organizationid:[Microsoft.Dynamics.CRM.organization Nullable=False]
        //ntt_debttype_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_debttype_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_debttype_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_debttype_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_debttype_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_debttype_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_debttype_ntt_creditordebttype_DebtTypeId:[Collection([Microsoft.Dynamics.CRM.ntt_creditordebttype Nullable=False]) Nullable=True]
    }
}
