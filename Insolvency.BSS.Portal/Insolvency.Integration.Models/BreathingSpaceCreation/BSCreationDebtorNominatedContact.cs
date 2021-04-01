using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Insolvency.Common.Attributes;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationDebtorNominatedContact : AbstractDebtorNominatedContactAddress<BSCreationAddress>
    {
        [JsonIgnore]
        [MultiConditionalRequired("Never", ErrorMessage = "Please enter confirm email address")]
        public override string ConfirmEmailAddress { get => base.EmailAddress; set { return; } }

        [JsonIgnore]
        public override Dictionary<string, Func<bool>> Actions
        {
            get
            {
                var result = base.Actions;
                result.Add("Never", () => false);
                return result;
            }
        }
    }
}
