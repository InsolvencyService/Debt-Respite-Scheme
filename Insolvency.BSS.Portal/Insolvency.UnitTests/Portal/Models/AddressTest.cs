using AutoFixture;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.Shared.Responses;
using Insolvency.Portal.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class AddressTest
    {
        public Fixture Fixture { get; }

        public AddressTest()
        {
            Fixture = new Fixture();
            Fixture.Customize<CustomAddress>(x =>
                x.With(p => p.Postcode, "EC3V 3DG")
                 .Without(p => p.OwnerId)
            );
        }

        [TestMethod]
        public void Address_Ctor_Should_Set_All_Value_Correctly()
        {
            // Arrange
            var model = Fixture.Create<AddressResponse>();

            // Act
            var result = new Address(model);

            // Assert
            Assert.AreEqual(result.AddressLine1, model.AddressLine1);
            Assert.AreEqual(result.AddressLine2, model.AddressLine2);
            Assert.AreEqual(result.TownCity, model.TownCity);
            Assert.AreEqual(result.County, model.County);
            Assert.AreEqual(result.Country, model.Country);
            Assert.AreEqual(result.Postcode, model.Postcode);
        }

        [TestMethod]
        public void PreviousAddress_Ctor_Should_Set_All_Value_Correctly()
        {
            // Arrange
            var model = Fixture.Create<PreviousAddressResponse>();

            // Act
            var result = new Address(model);

            // Assert
            Assert.AreEqual(result.AddressLine1, model.AddressLine1);
            Assert.AreEqual(result.AddressLine2, model.AddressLine2);
            Assert.AreEqual(result.TownCity, model.TownCity);
            Assert.AreEqual(result.County, model.County);
            Assert.AreEqual(result.Country, model.Country);
            Assert.AreEqual(result.Postcode, model.Postcode);
            Assert.AreEqual(result.DateFrom, model.DateFrom);
            Assert.AreEqual(result.DateTo, model.DateTo);
        }

        [TestMethod]
        public void Address_Must_Have_Empty_Ctor()
        {
            // Arrange
            var model = Fixture.Create<CustomAddress>();

            // Act
            var result = new Address
            {
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                TownCity = model.TownCity,
                County = model.County,
                Country = model.Country,
                Postcode = model.Postcode,
                DateFrom = model.DateFrom,
                DateTo = model.DateTo
            };

            // Assert
            Assert.AreEqual(result.AddressLine1, model.AddressLine1);
            Assert.AreEqual(result.AddressLine2, model.AddressLine2);
            Assert.AreEqual(result.TownCity, model.TownCity);
            Assert.AreEqual(result.County, model.County);
            Assert.AreEqual(result.Country, model.Country);
            Assert.AreEqual(result.Postcode, model.Postcode);
            Assert.AreEqual(result.DateFrom, model.DateFrom);
            Assert.AreEqual(result.DateTo, model.DateTo);
        }

        [TestMethod]
        public void Address_Should_Be_Equal()
        {
            // Arrange
            var model = Fixture.Create<Address>();
            var model1 = model;

            // Act
            var result = model.Equals(model1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Address_Should_Not_Be_Equal()
        {
            // Arrange
            var model = Fixture.Create<Address>();
            var model1 = Fixture.Create<Address>();

            // Act
            var result = model.Equals(model1);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
