namespace Insolvency.Portal.Models
{
    public class PartialAddress
    {
        public virtual string Address { get; set; }
        public virtual string Id { get; set; }

        public string ToViewString()
        {
            return Address;
        }
    }
}
