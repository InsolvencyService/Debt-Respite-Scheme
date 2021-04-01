using Insolvency.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal
{
    [TestClass]
    public class ContactPreferenceTypeTest
    {
        [TestMethod]
        public void EmailContactOption_Int_Value_Should_Match()
        {
            // Arrange
            ContactPreferenceType inputValue = ContactPreferenceType.Email;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void PostContactOption_Int_Value_Should_Match()
        {
            // Arrange
            ContactPreferenceType inputValue = ContactPreferenceType.Post;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(1, result);
        }
    }
}
