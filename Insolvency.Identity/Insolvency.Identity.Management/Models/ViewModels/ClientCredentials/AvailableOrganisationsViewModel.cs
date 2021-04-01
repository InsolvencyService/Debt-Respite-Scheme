using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Insolvency.Identity.Models;

namespace Insolvency.Management.Models.ViewModels.ClientCredentials
{
    public class AvailableOrganisationsViewModel
    {
        public List<Organisation> SelectableOrganisation { get; set; }
        [Required]
        public List<Guid> SelectedOrganisationIds { get; set; }

    }
}
