using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class CreditorSearchResultsViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public CreditorSearchResultsViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public CreditorSearchResultsViewModelTests()
        {
            this.Fixture = new Fixture();
            this.Sut = Fixture.Create<CreditorSearchResultsViewModel>();
            this.Messages = new List<ValidationResult>();
            this.ValidationContext = new ValidationContext(this.Sut);
        }

        [TestMethod]
        public void SelectedCreditor_Is_Required()
        {
            // Arrange
            Sut.SelectedCreditor = null;
            ValidationContext.MemberName = nameof(Sut.SelectedCreditor);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedCreditor, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SelectedCreditor_Is_Valid_If_Reason_Has_Value()
        {
            // Arrange
            Sut.SelectedCreditor = Fixture.Create<Guid>();
            ValidationContext.MemberName = nameof(Sut.SelectedCreditor);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedCreditor, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
