using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels.Creditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models.Creditor
{
    [TestClass]
    public class CreditorDebtEligibilityReviewViewModelTests
    {
        public Fixture Fixture { get; }
        public CreditorDebtEligibilityReviewViewModel Sut { get; }
        public List<ValidationResult> Messages { get; }
        public ValidationContext ValidationContext { get; }

        public CreditorDebtEligibilityReviewViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<CreditorDebtEligibilityReviewViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("            ")]
        public void CreditorDebtEligibilityReviewViewModel_CreditorNotes_Is_Required(string creditorNotes)
        {
            // Arrange
            Sut.CreditorNotes = creditorNotes;
            ValidationContext.MemberName = nameof(Sut.CreditorNotes);

            // Act
            var result = Validator.TryValidateProperty(Sut.CreditorNotes, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CreditorDebtEligibilityReviewViewModel_Reason_Is_Required()
        {
            // Arrange
            Sut.Reason = null;
            ValidationContext.MemberName = nameof(Sut.Reason);

            // Act
            var result = Validator.TryValidateProperty(Sut.Reason, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }
    }
}
