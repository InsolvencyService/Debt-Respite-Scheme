using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insolvency.Portal.Authentication
{
    public class PortalUser
    {
        public string SelectedOrganisationName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsAuthenticated { get; set; } = false;
    }
}
