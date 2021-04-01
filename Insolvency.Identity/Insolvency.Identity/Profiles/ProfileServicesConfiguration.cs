using IdentityServer4.Services;
using Insolvency.Identity.Extensions;
using Insolvency.Identity.Profiles.ProfileServices;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Identity.Profiles
{
    public static class ProfileServicesConfiguration
    {
        public static IIdentityServerBuilder ConfigureProfileServices(this IIdentityServerBuilder builder)
        {
            builder.AddProfileService<ScpProfileService>();
            builder.Services.AddComposite<IProfileService, CompositeProfileService>();
            return builder;
        }
    }
}
