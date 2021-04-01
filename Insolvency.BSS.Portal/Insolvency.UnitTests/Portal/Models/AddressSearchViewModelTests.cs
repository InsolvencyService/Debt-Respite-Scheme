using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class AddressSearchViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public AddressSearchViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public AddressSearchViewModelTests()
        {
            this.Fixture = new Fixture();
            this.Sut = Fixture.Create<AddressSearchViewModel>();
            this.Messages = new List<ValidationResult>();
            this.ValidationContext = new ValidationContext(this.Sut);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        public void SelectedAddress_Is_Required(string n)
        {
            // Arrange
            Sut.SelectedAddress = n;
            ValidationContext.MemberName = nameof(Sut.SelectedAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedAddress, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("123 fake street london se18fw")]
        [DataRow("87 westminster, london city, s12 945")]
        public void SelectedAddress_Is_Valid_If_SelectedAddress_Has_Value(string n)
        {
            // Arrange
            Sut.SelectedAddress = n;
            ValidationContext.MemberName = nameof(Sut.SelectedAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedAddress, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
