using System;
using Insolvency.Integration.Models;

namespace Insolvency.Portal.Models
{
    public class PreviousAddress : CustomAddress
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
