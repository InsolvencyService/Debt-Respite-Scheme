using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorPostcodeViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorPostcodeViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorPostcodeViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorPostcodeViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        public void Postcode_Cant_Be_Null()
        {
            // Arrange
            Sut.Postcode = null;
            ValidationContext.MemberName = nameof(Sut.Postcode);

            // Act
            var result = Validator.TryValidateProperty(Sut.Postcode, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Postcode_Cant_Be_Empty_String()
        {
            // Arrange
            Sut.Postcode = string.Empty;
            ValidationContext.MemberName = nameof(Sut.Postcode);

            // Act
            var result = Validator.TryValidateProperty(Sut.Postcode, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow("    ")]
        [DataRow("                  ")]
        public void Postcode_Cant_Be_Whitespace(string n)
        {
            // Arrange
            Sut.Postcode = n;
            ValidationContext.MemberName = nameof(Sut.Postcode);

            // Act
            var result = Validator.TryValidateProperty(Sut.Postcode, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("SE178FW")]
        [DataRow("abcdefghijkl")]
        [DataRow("!()*&^%$£")]
        public void Postcode_If_Valid_If_Has_Value(string n)
        {
            // Arrange
            Sut.Postcode = n;
            ValidationContext.MemberName = nameof(Sut.Postcode);

            // Act
            var result = Validator.TryValidateProperty(Sut.Postcode, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }
    }
}
