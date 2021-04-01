using System;
using System.Threading.Tasks;
using Insolvency.Integration.Models;
using Insolvency.Interfaces;
using Insolvency.Portal.Gateways;
using Insolvency.Portal.Interfaces;
using Insolvency.Portal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Insolvency.UnitTests
{
    [TestClass]
    public class CreditorAddressSubmitTest
    {
        private readonly IIntegrationGateway _integrationGateway;
        private readonly Mock<IApiClient> _mockApiClient;

        public CreditorAddressSubmitTest()
        {
            _mockApiClient = new Mock<IApiClient>();

            _integrationGateway = new IntegrationGateway(_mockApiClient.Object);
        }

        [TestMethod]
        public async Task ShouldAbleToSubmitCreditorData()
        {
            // Arrange
            var creditorAddress = new Address
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                TownCity = "TownCity",
                County = "County",
                Postcode = "Postcode"
            };

            var expected = Guid.NewGuid();
            _mockApiClient
                .Setup(s => s.CreateAsync<Guid, CustomAddress>(
                    It.IsAny<CustomAddress>(),
                    It.IsAny<string>())
                 )
                .Returns(Task.FromResult(expected));

            // Act
            var result = await _integrationGateway.AddAdHocCreditorAddressAsync(creditorAddress, expected);

            // Asserts
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task CheckClientApiBeingCalled()
        {
            // Arrange
            var creditorAddress = new Address
            {
                AddressLine1 = "AddressLine1",
                AddressLine2 = "AddressLine2",
                TownCity = "TownCity",
                County = "County",
                Postcode = "Postcode"
            };

            var expected = Guid.NewGuid();
            _mockApiClient
                .Setup(s => s.CreateAsync<Guid, CustomAddress>(
                    It.IsAny<CustomAddress>(),
                    It.IsAny<string>())
                 )
                .Returns(Task.FromResult(expected));

            // Act
            var result = await _integrationGateway.AddAdHocCreditorAddressAsync(creditorAddress, expected);

            // Asserts
            Assert.AreEqual(expected, result);

            // check client api being called
            _mockApiClient
                .Verify(v => v.CreateAsync<Guid, CustomAddress>(
                    It.IsAny<CustomAddress>(),
                    It.IsAny<string>()
                ),
                Times.Once);
        }
    }
}
