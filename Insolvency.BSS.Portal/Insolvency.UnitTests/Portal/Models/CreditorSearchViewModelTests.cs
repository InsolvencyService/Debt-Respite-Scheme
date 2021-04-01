using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class CreditorSearchViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public CreditorSearchViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public CreditorSearchViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<CreditorSearchViewModel>();
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
        [DataRow("CreditorName")]
        [DataRow("")]
        [DataRow(null)]
        public void Model_Must_Have_No_Parameters_Ctor(string creditorName)
        {
            // Arrange & Act
            var sut = new CreditorSearchViewModel
            {
                CreditorName = creditorName,
            };

            // Assert
            Assert.AreEqual(creditorName, sut.CreditorName);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void CreditorName_Is_Required(string creditorName)
        {
            // Arrange
            Sut.CreditorName = creditorName;
            ValidationContext.MemberName = nameof(Sut.CreditorName);

            // Act
            var result = Validator.TryValidateProperty(Sut.CreditorName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void CreditorName_Is_Valid_Up_To_100_Chars(int n)
        {
            // Arrange
            var creditorName = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.CreditorName = creditorName;
            ValidationContext.MemberName = nameof(Sut.CreditorName);

            // Act
            var result = Validator.TryValidateProperty(Sut.CreditorName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(101)]
        [DataRow(200)]
        public void CreditorName_Is_Invalid_Over_100_Chars(int n)
        {
            // Arrange
            var creditorName = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.CreditorName = creditorName;
            ValidationContext.MemberName = nameof(Sut.CreditorName);

            // Act
            var result = Validator.TryValidateProperty(Sut.CreditorName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }
    }
}
