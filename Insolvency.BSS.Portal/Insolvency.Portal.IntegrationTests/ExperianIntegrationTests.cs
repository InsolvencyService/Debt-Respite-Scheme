using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Portal.Gateways;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Portal.IntegrationTests
{
    [TestClass]
    public class ExperianIntegrationTests
    {
        public AddressLookupGateway Sut { get; }
       

        public ExperianIntegrationTests()
        {
            this.Sut = new AddressLookupGateway(Main.LookupClient);         
        }
        

        [DataTestMethod]
        [DataRow("E16 2UB")]
        [DataRow("TW88ER")]
        public async Task CreateClientAsync(string postcode)
        {
            // Arrange
            Main.SetMoneyAdviserCreditorApiCient();
            var model = Main.Fixture.Create<ClientDetailsCreateViewModel>();
            var gateway = new IntegrationGateway(Main.ApiClient);
            model.BirthYear = 1980;

            var validDate = false;
            while (!validDate)
            {
                try
                {
                    _ = new DateTime(model.BirthYear.Value, model.BirthMonth.Value, model.BirthDay.Value);
                    validDate = true;
                }
                catch
                {
                    model = Main.Fixture.Create<ClientDetailsCreateViewModel>();
                    model.BirthYear = 1980;
                }
            }

            var moratoriumId = await gateway.CreateClientAsync(model);

            var experianResults = await Sut.GetAddressesForPostcode(postcode);
            var partialAddress = experianResults.Addresses.First();
            var address = await Sut.GetFullAddressAsync(partialAddress.Id);

            // Act
            var addressId = await gateway.CreateDebtorAddressAsync(address, moratoriumId);

            // Assert
            Assert.AreNotEqual(default, addressId);
        }
    }
}
