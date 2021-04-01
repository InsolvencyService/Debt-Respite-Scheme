using System;
using System.Collections.Generic;
using System.Linq;
using Insolvency.Common.Enums;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;

namespace Insolvency.Integration.Gateways.Entities
{
    public class Ntt_breathingspacedebt : IDynamicsEntity
    {
        public virtual DateTimeOffset? ntt_retentionperiodstartdate { get; set; }
        public virtual string ntt_adviserexternalreferenceid { get; set; }
        public virtual DateTimeOffset? overriddencreatedon { get; set; }
        public virtual Guid? _createdby_value { get; set; }
        public virtual Guid? _owningbusinessunit_value { get; set; }
        public virtual DateTimeOffset? ntt_retentionperiodenddate { get; set; }
        public virtual decimal? ntt_amount_base { get; set; }
        public virtual Guid? _ntt_breathingspacemoratoriumid_value { get; set; }
        public virtual decimal? exchangerate { get; set; }
        public virtual Guid? _modifiedonbehalfby_value { get; set; }
        public virtual Guid? _ntt_debttype_value { get; set; }
        public virtual DateTimeOffset? modifiedon { get; set; }
        public virtual int? statecode { get; set; }
        public virtual DateTimeOffset? ntt_removedfromregisterdate { get; set; }
        public virtual Guid? _createdonbehalfby_value { get; set; }
        public virtual int? timezoneruleversionnumber { get; set; }
        public virtual Guid? _owninguser_value { get; set; }
        public virtual Guid? _owningteam_value { get; set; }
        public virtual string ntt_name { get; set; }
        public virtual DateTimeOffset? ntt_commencementdate { get; set; }
        public virtual DateTimeOffset? ntt_expirydate { get; set; }
        public virtual string ntt_reference { get; set; }
        public virtual long? versionnumber { get; set; }
        public virtual DateTimeOffset? createdon { get; set; }
        public virtual DateTimeOffset? ntt_removaldate { get; set; }
        public virtual int? importsequencenumber { get; set; }
        public virtual Guid? _ownerid_value { get; set; }
        public virtual Guid? ntt_breathingspacedebtid { get; set; }
        public virtual string ntt_nationalinsurancenumber { get; set; }
        public virtual DateTimeOffset? ntt_activeregisterperiodenddate { get; set; }
        public virtual int? statuscode { get; set; }
        public virtual Guid? _ntt_debtstatusid_value { get; set; }
        public virtual string ntt_creditorexternalreferenceid { get; set; }
        public virtual decimal? ntt_amount { get; set; }
        public virtual Guid? _modifiedby_value { get; set; }
        public virtual int? utcconversiontimezonecode { get; set; }
        public virtual Guid? _transactioncurrencyid_value { get; set; }
        public virtual bool? ntt_activeregister { get; set; }
        public virtual DateTimeOffset? ntt_activeregisterperiodstartdate { get; set; }
        public virtual string ntt_otherdebttype { get; set; }
        public virtual Guid? _ntt_soldtocreditorid_value { get; set; }
        public bool? ntt_previouslysold { get; set; }

        public Guid? _ntt_managingmoneyadviserorganisationid_value { get; set; }

        public Ntt_breathingspacecreditor ntt_creditorid { get; set; }
        public Guid _ntt_creditorid_value { get; set; }
        public Ntt_breathingspacecreditor ntt_SoldToCreditorId { get; set; }
        public INSS_debttype ntt_debttypeid { get; set; }
        public Ntt_breathingspacemoratorium ntt_BreathingSpaceMoratoriumId { get; set; }
        public List<ntt_debteligibilityreview> ntt_breathingspacedebt_ntt_debteligibilityreview_DebtId { get; set; }

        public Guid? _ntt_debtremovalreasonid_value { get; set; }

        public Guid GetId() => ntt_breathingspacedebtid.Value;

        public DebtStatus GetDebtStatus(DynamicsGatewayOptions options) => options.DebtStatus[_ntt_debtstatusid_value.ToString()];

        public static Guid GetDebtStatusValue(DynamicsGatewayOptions options, DebtStatus status)
        {
            return Guid.Parse(options.DebtStatus.First(p => p.Value == status).Key);
        }

        public DebtRemovalReason? GetDebtRemovalReason(DynamicsGatewayOptions options) => _ntt_debtremovalreasonid_value.HasValue ?
                    options.DebtRemovalReason.First(r => r.Value == _ntt_debtremovalreasonid_value.ToString()).Key :
                    (DebtRemovalReason?)null;

        public Debt ToDebtModel(DynamicsGatewayOptions options)
        {
            return new Debt
            {
                Id = GetId(),
                Amount = ntt_amount,
                Reference = ntt_creditorexternalreferenceid,
                DebtTypeName = ntt_debttypeid?.inss_name ?? ntt_otherdebttype,
                NINO = ntt_nationalinsurancenumber,
                CreatedOn = createdon.Value,
                RemovedOn = modifiedon.Value,
                Status = GetDebtStatus(options),
                SoldToCreditorName = ntt_SoldToCreditorId?.ntt_name,
                SoldToCreditorId = ntt_SoldToCreditorId?.GetId(),
                PreviouslySold = ntt_previouslysold ?? false,
                DebtRemovalReason = GetDebtRemovalReason(options)
            };
        }

        public DebtDetailResponse ToDebtDetail(DynamicsGatewayOptions options)
        {
            return new DebtDetailResponse
            {
                Id = GetId(),
                Amount = ntt_amount,
                Reference = ntt_creditorexternalreferenceid,
                DebtTypeName = ntt_debttypeid?.inss_name ?? ntt_otherdebttype,
                NINO = ntt_nationalinsurancenumber,
                CreatedOn = createdon.Value,
                StartsOn = ntt_commencementdate,
                ModifiedOn = modifiedon.Value,
                RemovedOn = ntt_removaldate,
                Status = GetDebtStatus(options),
                SoldToCreditorName = ntt_SoldToCreditorId?.ntt_name,
                SoldToCreditorId = ntt_SoldToCreditorId?.GetId(),
                PreviouslySold = ntt_previouslysold ?? false,
                CreditorName = ntt_creditorid?.ntt_name,
                CreditorId = ntt_creditorid?.ntt_breathingspacecreditorid.Value,
                DebtEligibilityReview = ntt_breathingspacedebt_ntt_debteligibilityreview_DebtId
                    ?.FirstOrDefault() // we can have only one active debt review
                    ?.ToDebtEligibilityReview(options),
                DebtRemovalReason = GetDebtRemovalReason(options)
            };
        }

        // Other properties from the model
        //createdby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //createdonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //modifiedonbehalfby:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owninguser:[Microsoft.Dynamics.CRM.systemuser Nullable=False]
        //owningteam:[Microsoft.Dynamics.CRM.team Nullable=False]
        //ownerid:[Microsoft.Dynamics.CRM.principal Nullable=False]
        //owningbusinessunit:[Microsoft.Dynamics.CRM.businessunit Nullable=False]
        //ntt_breathingspacedebt_SyncErrors:[Collection([Microsoft.Dynamics.CRM.syncerror Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_AsyncOperations:[Collection([Microsoft.Dynamics.CRM.asyncoperation Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_MailboxTrackingFolders:[Collection([Microsoft.Dynamics.CRM.mailboxtrackingfolder Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_ProcessSession:[Collection([Microsoft.Dynamics.CRM.processsession Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_BulkDeleteFailures:[Collection([Microsoft.Dynamics.CRM.bulkdeletefailure Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_PrincipalObjectAttributeAccesses:[Collection([Microsoft.Dynamics.CRM.principalobjectattributeaccess Nullable=False]) Nullable=True]
        //ntt_breathingspacedebt_Annotations:[Collection([Microsoft.Dynamics.CRM.annotation Nullable=False]) Nullable=True]
        //transactioncurrencyid:[Microsoft.Dynamics.CRM.transactioncurrency Nullable=False]
        //ntt_debtstatusid:[Microsoft.Dynamics.CRM.ntt_breathingspacedebtstatustype Nullable=False]
        //ntt_DebtType:[Microsoft.Dynamics.CRM.inss_debttype Nullable=False]

        // How to get
        //var foo = await _client.GetMetadataAsync<EdmModelBase>();
        //var t = (IEdmEntityType)foo.SchemaElements.Single(x => x.FullName() == "Microsoft.Dynamics.CRM.ntt_debtoreligibilityreview");
        //var z = t.DeclaredProperties.Aggregate("", (x, y) => $"{x}{y.Name}:{y.Type}\n");
    }
}
