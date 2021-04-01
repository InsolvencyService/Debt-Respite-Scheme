using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Common.Identity.Claims.Types;

namespace Insolvency.Portal.Models
{
    public class OrganisationModel
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string OrganisationTypeName { get; set; }

        public static OrganisationModel FromClaimType(OrganisationClaimType claimType)
        {
            return new OrganisationModel
            {
                Id = Guid.Parse(claimType.Id),
                Name = claimType.Name,
                OrganisationTypeName = claimType.OrganisationTypeName
            };
        }
    }
}
