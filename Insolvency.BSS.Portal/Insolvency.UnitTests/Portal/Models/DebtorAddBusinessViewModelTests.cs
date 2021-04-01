using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using Insolvency.Portal.Models;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorAddBusinessViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorAddBusinessViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorAddBusinessViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorAddBusinessViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        public void Model_IsValid()
        {
            // Arrange & Act
            var result = Validator.TryValidateObject(Sut, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("BusinessName", "Yes", true)]
        [DataRow("BusinessName", "Yes", false)]
        [DataRow("BusinessName", "No", true)]
        [DataRow("BusinessName", "No", false)]
        public void Model_Must_Have_No_Parameters_Ctor(string businessName, string isAddressSameAsCurrent, bool displayHideAddressLabel)
        {
            // Arrange & Act
            var moratoriumId = Fixture.Create<Guid>();
            var currentAddress = Fixture.Create<Address>();
            var sut = new DebtorAddBusinessViewModel {
                BusinessName = businessName,
                IsAddressSameAsCurrent = isAddressSameAsCurrent,
                DisplayHideAddressLabel = displayHideAddressLabel,
                MoratoriumId = moratoriumId,
                DebtorCurrentAddress = currentAddress,
            };

            // Assert
            Assert.AreEqual(businessName, sut.BusinessName);
            Assert.AreEqual(moratoriumId, sut.MoratoriumId);
            Assert.AreEqual(currentAddress, sut.DebtorCurrentAddress);
            Assert.AreEqual(displayHideAddressLabel, sut.DisplayHideAddressLabel);
            Assert.AreEqual(isAddressSameAsCurrent, sut.IsAddressSameAsCurrent);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void IsAddressSameAsCurrent_Is_Required(string isAddressSameAsCurrent)
        {
            // Arrange
            Sut.IsAddressSameAsCurrent = isAddressSameAsCurrent;
            ValidationContext.MemberName = nameof(Sut.IsAddressSameAsCurrent);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsAddressSameAsCurrent, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
            Assert.IsTrue(Messages.FindAll(errorMessage => String.Equals("Please select an option", errorMessage.ErrorMessage)).Count == 1);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void BusinessName_Is_Required(string businessName)
        {
            // Arrange
            Sut.BusinessName = businessName;
            ValidationContext.MemberName = nameof(Sut.BusinessName);

            // Act
            var result = Validator.TryValidateProperty(Sut.BusinessName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void BusinessName_Is_Valid_Up_To_100_Chars(int n)
        {
            // Arrange
            var businessName = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.BusinessName = businessName;
            ValidationContext.MemberName = nameof(Sut.BusinessName);

            // Act
            var result = Validator.TryValidateProperty(Sut.BusinessName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(101)]
        [DataRow(200)]
        public void BusinessName_Is_Invalid_Over_100_Chars(int n)
        {
            // Arrange
            var businessName = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.BusinessName = businessName;
            ValidationContext.MemberName = nameof(Sut.BusinessName);

            // Act
            var result = Validator.TryValidateProperty(Sut.BusinessName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("Yes")]
        public void When_IsAddressSameAsCurrent_Is_Yes_Then_UseCurrentAddress_Is_True(string isAddressSameAsCurrent)
        {
            // Arrange
            Sut.IsAddressSameAsCurrent = isAddressSameAsCurrent;

            // Act
            var result = Sut.UseCurrentAddress;

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("yes")]
        [DataRow("No")]
        [DataRow("")]
        public void When_IsAddressSameAsCurrent_Is_Not_Yes_Then_UseCurrentAddress_Is_false(string isAddressSameAsCurrent)
        {
            // Arrange
            Sut.IsAddressSameAsCurrent = isAddressSameAsCurrent;

            // Act
            var result = Sut.UseCurrentAddress;

            // Assert     
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void ToBusinessModel_Returns_Expected_Value()
        {
            // Arrange
            var businessName = Sut.BusinessName;
            var address = Sut.DebtorCurrentAddress.ToCustomAddress();
            var moratoriumId = Sut.MoratoriumId;

            // Act
            var result = Sut.ToCreateBusinessModel();

            // Assert     
            Assert.AreEqual(address, result.Address);
            Assert.AreEqual(businessName, result.BusinessName);
            Assert.AreEqual(moratoriumId, result.Address.OwnerId);
        }

        [TestMethod]
        public void BusinessName_DisplayName_Should_Match()
        {
            // Arrange
            ValidationContext.MemberName = nameof(Sut.BusinessName);

            // Act
            var result = ValidationContext.DisplayName;

            // Assert     
            Assert.AreEqual("Enter the client's business or trading name", result);
        }
    }
}
