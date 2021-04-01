using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insolvency.Portal.Models
{
    public class SelectOrganisationViewModel
    {
        public List<OrganisationModel> Organisations { get; set; }

        public OrganisationModel CurrentSelectedOrganisation { get; set; }

        public string ReturnUrl { get; set; }

        public bool RedirectedFromMiddlewear { get; set; }
    }
}
