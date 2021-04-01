using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Gateways.MoratoriumEntities
{
    public class MoratoriumDetail
    {
        public Guid Id { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedOn { get; set; }
        public string Status { get; set; }
        public Guid StatusId { get; set; }
        public string Type { get; set; }
        public Guid TypeId { get; set; }
        public bool IsMentalHealth { get; set; }
        public bool IsAddressWithheld { get; set; }
        public string CommencementDate { get; set; }
        public string ExpiryDate { get; set; }
        public string ClosureDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string ParentCancellationReason { get; set; }
        public Debtor Debtor { get; set; }
        public MoneyAdviserOrganisation MoneyAdviserOrganisation { get; set; }
        public IEnumerable<Debt> Debts { get; set; }
        public IEnumerable<DebtorEligibilityReview> DebtorEligibilityReview { get; set; }
        public IEnumerable<ExternalContactParent> ExternalContacts { get; set; }
        public MoratoriumTransfer MoratoriumTransfer { get; set; }
    }
}