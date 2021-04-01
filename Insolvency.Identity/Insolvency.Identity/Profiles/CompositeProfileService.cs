using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Insolvency.Identity.Profiles
{
    public class CompositeProfileService : DefaultProfileService
    {
        private readonly IEnumerable<IProfileService> _profileServices;

        public CompositeProfileService(
            ILogger<DefaultProfileService> logger,
            IEnumerable<IProfileService> profileServices)
            :base(logger)
        {
            _profileServices = profileServices;
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {            
            return Task.WhenAll(_profileServices.Select(ps => ps.GetProfileDataAsync(context)));         
        }       
    }
}
