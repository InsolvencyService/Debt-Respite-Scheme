using System;

namespace Insolvency.Integration.Models
{
    public class UpdateCustomAddress : CustomAddress
    {        
        public virtual Guid AddressId { get; set; }

        public virtual Guid MoratoriumId { get; set; }
    }
}