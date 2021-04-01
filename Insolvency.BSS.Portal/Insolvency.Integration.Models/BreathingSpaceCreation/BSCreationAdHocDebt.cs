using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Insolvency.Common.Enums;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationAdHocDebt : BSCreationDebt
    {
        [JsonIgnore]
        public override Guid? DebtTypeId { get; set; }

        [JsonIgnore]
        public override Guid CreditorId { get; set; }

        [Required]
        public string CreditorName { get; set; }

        [Required]
        public BSCreationAddress Address { get; set; }

        [JsonIgnore]
        public override DebtStatus Status { get; set; }

        [JsonIgnore]
        public override Guid? SoldToCreditorId { get; set; }

        [JsonIgnore]
        public override string SoldToCreditorName { get; set; }

        [JsonIgnore]
        public override bool PreviouslySold { get; set; }

        public override Dictionary<string, object> ToDictionary()
        {
            var baseResult = base.ToDictionary();
            baseResult.Add("Name", CreditorName);
            baseResult.Add("Address", Address.ToDictionary());
            if (baseResult.ContainsKey("CreditorId"))
            {
                baseResult.Remove("CreditorId");
            }
            if (baseResult.ContainsKey("DebtTypeId"))
            {
                baseResult.Remove("DebtTypeId");
            }
            return baseResult;
        }
    }
}
