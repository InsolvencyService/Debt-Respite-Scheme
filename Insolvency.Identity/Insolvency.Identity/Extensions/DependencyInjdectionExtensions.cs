using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Identity.Extensions
{
    public static class DependencyInjdectionExtensions
    {
        public static void AddComposite<TInterface, TConcrete>(this IServiceCollection services)
            where TInterface : class
            where TConcrete : class, TInterface
        {
            var wrappedDescriptors = services.Where(s => s.ServiceType == typeof(TInterface)).ToList();
            foreach (var descriptor in wrappedDescriptors)
                services.Remove(descriptor);

            var objectFactory = ActivatorUtilities.CreateFactory(
              typeof(TConcrete),
              new[] { typeof(IEnumerable<TInterface>) });

            services.Add(ServiceDescriptor.Describe(
              typeof(TInterface),
              s => (TInterface)objectFactory(s, new[] { wrappedDescriptors.Select(d => ActivatorUtilities.CreateInstance(s, d.ImplementationType)).Cast<TInterface>() }),
              wrappedDescriptors.Select(d => d.Lifetime).Max())
            );
        }
    }
}
