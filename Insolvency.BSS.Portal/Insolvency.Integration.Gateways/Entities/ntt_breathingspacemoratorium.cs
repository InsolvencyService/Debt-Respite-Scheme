using System;
using System.Collections.Generic;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Gateways.Entities
{
    public abstract class Abstract_Ntt_breathingspacemoratorium : IDynamicsEntity
    {
        public DateTimeOffset? ntt_retentionperiodstartdate { get; set; }
        public string ntt_adviserexternalreferenceid { get; set; }
        public DateTimeOffset? overriddencreatedon { get; set; }
        public DateTimeOffset? ntt_retentionperiodenddate { get; set; }
        public decimal? ntt_amount_base { get; set; }
        public decimal? exchangerate { get; set; }
        public DateTimeOffset? modifiedon { get; set; }
        public int? statecode { get; set; }
        public DateTimeOffset? ntt_removedfromregisterdate { get; set; }
        public int? timezoneruleversionnumber { get; set; }
        public string ntt_name { get; set; }
        public DateTimeOffset? ntt_commencementdate { get; set; }
        public DateTimeOffset? ntt_expirydate { get; set; }
        public DateTimeOffset? ntt_closuredate { get; set; }
        public string ntt_referencenumber { get; set; }
        public long? versionnumber { get; set; }
        public DateTimeOffset? createdon { get; set; }
        public int? importsequencenumber { get; set; }
        public Guid? ntt_breathingspacedebtid { get; set; }
        public Guid? _ntt_managingmoneyadviserorganisationid_value { get; set; }
        public string ntt_nationalinsurancenumber { get; set; }
        public DateTimeOffset? ntt_activeregisterperiodenddate { get; set; }
        public int? statuscode { get; set; }
        public string ntt_creditorexternalreferenceid { get; set; }
        public decimal? ntt_amount { get; set; }
        public int? utcconversiontimezonecode { get; set; }
        public bool? ntt_activeregister { get; set; }
        public DateTimeOffset? ntt_activeregisterperiodstartdate { get; set; }
        public Ntt_breathingspacestatustype ntt_breathingspacestatusid { get; set; }

        public Ntt_breathingspacetype ntt_breathingspacetypeid { get; set; }

        public Guid? _ntt_breathingspacestatusid_value { get; set; }

        public Ntt_breathingspacedebtor ntt_debtorid { get; set; }

        public inss_moneyadviserorganisation ntt_ManagingMoneyAdviserOrganisationId { get; set; }

        public List<ntt_debtoreligibilityreview> ntt_breathingspacemoratorium_ntt_debtoreligibilityreview_MoratoriumId { get; set; }

        public List<Ntt_breathingspacedebt> ntt_breathingspacemoratorium_ntt_breathingspacedebt_BreathingSpaceMoratoriumId { get; set; }

        public abstract Guid GetId();
        public Guid? _ntt_debtorid_value { get; set; }

        //createdby[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //ntt_breathingspacedebt_SyncErrors[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_AsyncOperations[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_MailboxTrackingFolders[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_ProcessSession[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_BulkDeleteFailures[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_PrincipalObjectAttributeAccesses[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_Annotations[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //transactioncurrencyid[Microsoft.Dynamics.CRM.transactioncurrency Nullable=False]
        //ntt_debtstatusid[Microsoft.Dynamics.CRM.ntt_breathingspacedebtstatustype Nullable=False]
        //ntt_DebtType[Microsoft.Dynamics.CRM.inss_debttype Nullable=False]

        public DateTimeOffset? GetMoratoriumEndDate()
        {
            var status = GetMoratoriumStatus();

            if (status == MoratoriumStatus.Cancelled)
                return ntt_closuredate;

            return ntt_expirydate;
        }

        public MoratoriumStatus GetMoratoriumStatus()
        {
            return MoratoriumIdStatusMap.GetStatusFromId(_ntt_breathingspacestatusid_value);
        }
    }
}
