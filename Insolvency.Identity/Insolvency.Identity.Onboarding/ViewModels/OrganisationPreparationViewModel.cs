using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Insolvency.Identity.Models;

namespace Insolvency.Identity.Onboarding.ViewModels
{
    public class OrganisationPreparationViewModel
    {
        [Required]
        public string OrganisationName { get; set; }

        [Required]
        public string OrganisationId { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string OrganisationTypeName { get; set; }

        [Required]
        public IEnumerable<string> Scopes { get; set; }

        public Organisation MapToOrganisationEntity()
        {            
            return new Organisation()
            {                
                Name = OrganisationName,
                OnboarderEmail = EmailAddress,
                Type = OrganisationTypeName,
                ExternalId = OrganisationId,
                Scopes = Scopes
                         .Select(s => new OrganisationScope()
                         {
                             Name = s
                         }).ToList(),
                RoleUsers = new List<RoleUser>()
                {
                    new RoleUser { Email = EmailAddress, NormalisedEmail = EmailAddress.ToUpper(), Role = RoleType.Administrator  }
                }
            };
        }
    }
}
