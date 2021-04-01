using System;

namespace Insolvency.Integration.Gateways.Entities
{
    class ntt_moratoriumtransferrequest : IDynamicsEntity
    {
        public virtual DateTimeOffset? createdon { get; set; }
        public virtual DateTimeOffset? modifiedon { get; set; }
        public virtual inss_moneyadviserorganisation? ntt_owningmoneyadviceorganisationid { get; set; }
        public virtual string ntt_name { get; set; }
        public virtual string ntt_reasonfortransfer { get; set; }
        public virtual Guid? ntt_moratoriumtransferrequestid { get; set; }
        public virtual Ntt_breathingspacemoratorium? ntt_breathingspacemoratoriumid { get; set; }
        public virtual DateTimeOffset? ntt_acknowledgedon { get; set; }
        public virtual string ntt_acknowledgingmoneyadvisername { get; set; }
        public virtual DateTimeOffset? ntt_completedon { get; set; }
        public virtual string ntt_completingmoneyadvisername { get; set; }
        public virtual inss_moneyadviserorganisation? ntt_requestingmoneyadviceorganisationid { get; set; }
        public virtual string ntt_requestingmoneyadvisername { get; set; }
        public virtual DateTimeOffset? ntt_requestedon { get; set; }
        public virtual int statuscode { get; set; }
        public Guid GetId() => ntt_moratoriumtransferrequestid.Value;
        //Microsoft.Xrm.Sdk.EntityReference CreatedBy;
        //System.Nullable<System.DateTime> CreatedOn;
        //Microsoft.Xrm.Sdk.EntityReference CreatedOnBehalfBy;
        //System.Nullable<int> ImportSequenceNumber;
        //Microsoft.Xrm.Sdk.EntityReference ModifiedBy;
        //System.Nullable<System.DateTime> ModifiedOn;
        //Microsoft.Xrm.Sdk.EntityReference ModifiedOnBehalfBy;
        //System.Nullable<System.DateTime> ntt_AcknowledgedOn;
        //string ntt_AcknowledgingMoneyAdviserName;
        //Microsoft.Xrm.Sdk.EntityReference ntt_BreathingSpaceMoratoriumId;
        //System.Nullable<System.DateTime> ntt_CompletedOn;
        //string ntt_CompletingMoneyAdviserName;
        //string ntt_ModifiedByClientId;
        //string ntt_ModifiedByCreditorOrganisation;
        //string ntt_ModifiedByEmailAddress;
        //string ntt_ModifiedByMoneyAdviserOrganisation;
        //string ntt_ModifiedByName;
        //System.Nullable<System.Guid> ntt_moratoriumtransferrequestId;
        //override System.Guid Id;
        //string ntt_name;
        //Microsoft.Xrm.Sdk.EntityReference ntt_owningmoneyadviceorganisationid;
        //string ntt_ReasonforTransfer;
        //System.Nullable<System.DateTime> ntt_RequestedOn;
        //Microsoft.Xrm.Sdk.EntityReference ntt_requestingmoneyadviceorganisationid;
        //string ntt_RequestingMoneyAdviserName;
        //System.Nullable<System.DateTime> OverriddenCreatedOn;
        //Microsoft.Xrm.Sdk.EntityReference OwnerId;
        //Microsoft.Xrm.Sdk.EntityReference OwningBusinessUnit;
        //Microsoft.Xrm.Sdk.EntityReference OwningTeam;
        //Microsoft.Xrm.Sdk.EntityReference OwningUser;
        //System.Nullable<CMS.BSS.Plugin.MoneyAdviserOrganisation.Entities.ntt_moratoriumtransferrequestState> StateCode;
        //virtual ntt_moratoriumtransferrequest_StatusCode? StatusCode;
        //System.Nullable<int> TimeZoneRuleVersionNumber;
        //System.Nullable<int> UTCConversionTimeZoneCode;
        //System.Nullable<long> VersionNumber;
        //The value of the statuscode field:
        //Requested = 1,
        //Completed = 2,
        //Transferred = 961080000
    }
}
