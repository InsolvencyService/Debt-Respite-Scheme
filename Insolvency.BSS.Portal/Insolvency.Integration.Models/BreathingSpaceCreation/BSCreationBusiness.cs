using System.ComponentModel.DataAnnotations;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationBusiness : AbstractBusinessAddress<BSCreationAddress>
    {
        [Required]
        public override BSCreationAddress Address { get; set; }
    }
}
