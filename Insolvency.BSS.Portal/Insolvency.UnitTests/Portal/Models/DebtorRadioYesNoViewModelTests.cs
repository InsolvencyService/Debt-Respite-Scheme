using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorRadioYesNoViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorRadioYesNoViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorRadioYesNoViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorRadioYesNoViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        public void SubmitOption_Cant_Be_Null()
        {
            // Arrange
            Sut.SubmitOption = null;
            ValidationContext.MemberName = nameof(Sut.SubmitOption);

            // Act
            var result = Validator.TryValidateProperty(Sut.SubmitOption, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SubmitOption_Cant_Be_Empty_String()
        {
            // Arrange
            Sut.SubmitOption = string.Empty;
            ValidationContext.MemberName = nameof(Sut.SubmitOption);

            // Act
            var result = Validator.TryValidateProperty(Sut.SubmitOption, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("Yes")]
        [DataRow("No")]
        [DataRow("YES")]
        [DataRow("NO")]
        public void SubmitOption_Is_Valid_If_It_Has_Value(string n)
        {
            // Arrange
            Sut.SubmitOption = n;
            ValidationContext.MemberName = nameof(Sut.SubmitOption);

            // Act
            var result = Validator.TryValidateProperty(Sut.SubmitOption, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }
    }
}
