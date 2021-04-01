using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationDebt : Debt
    {
        [JsonIgnore]
        public override Guid MoratoriumId { get; set; }

        [JsonIgnore]
        public override Guid Id { get; set; }

        [Required]
        public override Guid CreditorId { get; set; }

        [JsonIgnore]
        public override DateTimeOffset CreatedOn { get; set; }
        [JsonIgnore]
        public override DateTimeOffset RemovedOn { get; set; }
        [JsonIgnore]
        public override DebtStatus Status { get; set; }
        [JsonIgnore]
        public override Guid? SoldToCreditorId { get; set; }
        [JsonIgnore]
        public override string SoldToCreditorName { get; set; }
        [JsonIgnore]
        public override bool PreviouslySold { get; set; }
    }
}
