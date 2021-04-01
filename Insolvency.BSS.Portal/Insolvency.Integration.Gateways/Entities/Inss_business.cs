using System;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Inss_business : IDynamicsEntity
    {
        public Guid? inss_businessid { get; set; }
        public string inss_name { get; set; }
        public int inss_businesstype { get; set; }
        public Guid? _inss_tradingaddressid_value { get; set; }
        public Inss_InssAddress inss_TradingAddressId { get; set; }
        public DateTimeOffset? createdon { get; set; }

        public Guid GetId() => inss_businessid.Value;

        //public List<Ntt_breathingspacedebtor> ntt_business_ntt_breathingspacedebtor_BusinessId { get; set; }

        //_owningbusinessunit_value:[Edm.Guid Nullable=True]
        //_ownerid_value:[Edm.Guid Nullable=True]
        //modifiedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //importsequencenumber:[Edm.Int32 Nullable=True]
        //_modifiedonbehalfby_value:[Edm.Guid Nullable=True]
        //_createdby_value:[Edm.Guid Nullable=True]
        //statuscode:[Edm.Int32 Nullable=True]
        //_owningteam_value:[Edm.Guid Nullable=True]
        //_createdonbehalfby_value:[Edm.Guid Nullable=True]
        //_owninguser_value:[Edm.Guid Nullable=True]
        //overriddencreatedon:[Edm.DateTimeOffset Nullable=True Precision=0]
        //_modifiedby_value:[Edm.Guid Nullable=True]
        //statecode:[Edm.Int32 Nullable=True]
        //versionnumber:[Edm.Int64 Nullable=True]
        //timezoneruleversionnumber:[Edm.Int32 Nullable=True]

        //utcconversiontimezonecode:[Edm.Int32 Nullable=True]
        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //inss_business_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //inss_business_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //inss_business_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //inss_business_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //inss_business_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //inss_business_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //inss_business_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
    }
}