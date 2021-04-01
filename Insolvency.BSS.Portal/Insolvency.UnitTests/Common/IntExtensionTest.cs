using Insolvency.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Common
{
    [TestClass]
    public class IntExtensionTest
    {
        [TestMethod]
        [DataRow(11)]
        [DataRow(19)]
        [DataRow(18)]
        public void Int_Ordinal_Should_Be_Th(int n)
        {
            // Arrange

            // Act
            var result = n.ToOrdinal();

            // Assert
            Assert.AreEqual("th", result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(21)]
        [DataRow(31)]
        public void Int_Ordinal_Should_Be_St(int n)
        {
            // Arrange

            // Act
            var result = n.ToOrdinal();

            // Assert
            Assert.AreEqual("st", result);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(22)]
        public void Int_Ordinal_Should_Be_Nd(int n)
        {
            // Arrange

            // Act
            var result = n.ToOrdinal();

            // Assert
            Assert.AreEqual("nd", result);
        }

        [TestMethod]
        [DataRow(3)]
        [DataRow(23)]
        public void Int_Ordinal_Should_Be_Rd(int n)
        {
            // Arrange

            // Act
            var result = n.ToOrdinal();

            // Assert
            Assert.AreEqual("rd", result);
        }
    }
}
