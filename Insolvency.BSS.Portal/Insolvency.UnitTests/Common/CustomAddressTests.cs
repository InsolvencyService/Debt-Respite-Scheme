using System;
using Insolvency.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Common
{
    [TestClass]
    public class CustomAddressTests
    {
        [TestMethod]
        public void CustomAddress_EqualsSameData_ReturnsTrue()
        {
            var ownerId = Guid.NewGuid();

            var addr1 = new CustomAddress()
            {
                AddressLine1 = "Line1",
                AddressLine2 = "Line2",
                Country = "Country",
                County = "County",
                DateFrom = new DateTime(1990, 1, 1),
                DateTo = new DateTime(1991, 1, 1),
                OwnerId = ownerId,
                Postcode = "SW1 1AA",
                TownCity = "City"
            };

            var addr2 = new CustomAddress()
            {
                AddressLine1 = "Line1",
                AddressLine2 = "Line2",
                Country = "Country",
                County = "County",
                DateFrom = new DateTime(1990, 1, 1),
                DateTo = new DateTime(1991, 1, 1),
                OwnerId = ownerId,
                Postcode = "SW1 1AA",
                TownCity = "City"
            };

            Assert.IsTrue(addr1.Equals(addr2));
        }

        [TestMethod]
        public void CustomAddressCurrent_EqualsSameData_ReturnsTrue()
        {
            var addr1 = new UpdateCustomAddressCurrent()
            {
                AddressLine1 = "Line1",
                AddressLine2 = "Line2",
                Country = "Country",
                County = "County",               
                Postcode = "SW1 1AA",
                TownCity = "City"
            };

            var addr2 = new UpdateCustomAddressCurrent()
            {
                AddressLine1 = "Line1",
                AddressLine2 = "Line2",
                Country = "Country",
                County = "County",               
                Postcode = "SW1 1AA",
                TownCity = "City"
            };

            Assert.IsTrue(addr1.Equals(addr2));
        }
    }
}
