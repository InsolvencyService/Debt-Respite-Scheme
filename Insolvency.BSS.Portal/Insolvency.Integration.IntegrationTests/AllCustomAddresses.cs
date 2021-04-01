using AutoFixture;
using Insolvency.Integration.Models;

namespace Insolvency.Integration.IntegrationTests
{
    public class AllCustomAddresses : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CustomAddress>(x =>
                x.With(p => p.Postcode, "EC3V 3DG")
                .Without(x => x.OwnerId));
        }
    }
}
