using System;
using System.Collections.Generic;

namespace Insolvency.Integration.Models.BreathingSpaceCreation
{
    public class BSCreationResponse
    {
        public string BreathingSpaceReference { get; set; }
        public Guid BreathingSpaceId { get; set; }
        public List<BSCreationResponseDebts> BreathingSpaceDebts { get; set; }
    }
}




