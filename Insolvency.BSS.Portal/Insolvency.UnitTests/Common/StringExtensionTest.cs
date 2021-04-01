using System.Globalization;
using System.Threading;
using Insolvency.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Common
{
    [TestClass]
    public class StringExtensionTest
    {
        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
        }

        [TestMethod]
        [DataRow("31/12/2020")]
        [DataRow("01/12/2020")]
        public void Value_Should_Be_Converted_To_Valid_DateTimeOffset(string v)
        {
            // Arrange

            // Act
            var result = v.ToDateTimeOffset();

            // Assert
            Assert.IsTrue(result != default);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("1000/1200/2020")]
        [DataRow("1/1/2020")]
        [DataRow(null)]
        public void Invalid_Input_Should_Not_Throw_Exception(string v)
        {
            // Arrange

            // Act
            var result = v.ToDateTimeOffset();

            // Assert
            Assert.IsTrue(result == default);
        }

        [TestMethod]
        [DataRow("1/1/2020")]
        public void Value_Should_Be_Formatted_And_Converted_To_Valid_DateTimeOffset(string v)
        {
            // Arrange

            // Act
            var result = v.ToDateTimeOffset("d/M/yyyy");

            // Assert
            Assert.IsTrue(result != default);
        }
    }
}
