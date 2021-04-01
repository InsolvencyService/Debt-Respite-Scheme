using System.Collections.Generic;
namespace Insolvency.Portal.Models
{
    public class SetSessionModel
    {
        public string RedirectUrl { get; set; }
        public IList<SessionValueModel> Values { get; set; }
    }
}
