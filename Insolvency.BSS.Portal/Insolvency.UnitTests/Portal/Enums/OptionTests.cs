using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Enums
{
    [TestClass]
    public class OptionTests
    {
        [TestMethod]
        public void YesOption_Int_Value_Should_Match()
        {
            // Arrange
            Option inputValue = Option.Yes;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NoOption_Int_Value_Should_Match()
        {
            // Arrange
            Option inputValue = Option.No;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(2, result);
        }
    }
}
