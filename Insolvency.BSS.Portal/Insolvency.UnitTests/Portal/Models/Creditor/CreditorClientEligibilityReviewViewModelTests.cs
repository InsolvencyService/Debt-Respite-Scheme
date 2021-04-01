using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Portal.Models.ViewModels.Creditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models.Creditor
{
    [TestClass]
    public class CreditorClientEligibilityReviewViewModelTests
    {
        public Fixture Fixture { get; }
        public CreditorClientEligibilityReviewViewModel Sut { get; }
        public List<ValidationResult> Messages { get; }
        public ValidationContext ValidationContext { get; }

        public CreditorClientEligibilityReviewViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<CreditorClientEligibilityReviewViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("            ")]
        public void CreditorClientEligibilityReviewViewModel_CreditorNotes_Is_Required(string creditorNotes)
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
        public void CreditorClientEligibilityReviewViewModel_EndReason_Is_Required()
        {
            // Arrange
            Sut.EndResaon = null;
            ValidationContext.MemberName = nameof(Sut.EndResaon);

            // Act
            var result = Validator.TryValidateProperty(Sut.EndResaon, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CreditorClientEligibilityReviewViewModel_NoLongerEligibleReason_Is_Only_Required_When_EndReason_Is_NoLongerEligible()
        {
            // Arrange
            Sut.EndResaon = BreathingSpaceClientEndReasonType.NoLongerEligible;
            Sut.NoLongerEligibleReason = null;
            ValidationContext.MemberName = nameof(Sut.NoLongerEligibleReason);

            // Act
            var result = Validator.TryValidateProperty(Sut.NoLongerEligibleReason, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CreditorClientEligibilityReviewViewModel_NoLongerEligibleReason_Is_Not_Required_When_EndReason_Is_Not_NoLongerEligible()
        {
            // Arrange
            Sut.EndResaon = BreathingSpaceClientEndReasonType.AbleToPayDebts;
            Sut.NoLongerEligibleReason = null;
            ValidationContext.MemberName = nameof(Sut.NoLongerEligibleReason);

            // Act
            var result = Validator.TryValidateProperty(Sut.NoLongerEligibleReason, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }
    }
}
