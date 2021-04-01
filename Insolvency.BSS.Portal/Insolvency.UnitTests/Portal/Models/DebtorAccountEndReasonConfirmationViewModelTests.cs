using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorAccountEndReasonConfirmationViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorAccountEndReasonConfirmationViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorAccountEndReasonConfirmationViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorAccountEndReasonConfirmationViewModel>();
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
        [DataRow(false)]
        [DataRow(true)]
        public void Model_IsInMentalHealthMoratorium_Can_Be_True_And_False(bool isInMentalHealthMoratorium)
        {
            // Arrange
            Sut.IsInMentalHealthMoratorium = isInMentalHealthMoratorium;
            ValidationContext.MemberName = nameof(Sut.IsInMentalHealthMoratorium);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsInMentalHealthMoratorium, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(BreathingSpaceEndReasonType.AbleToPayDebts)]
        [DataRow(BreathingSpaceEndReasonType.Cancelled)]
        [DataRow(BreathingSpaceEndReasonType.DebtManagementSolution)]
        [DataRow(BreathingSpaceEndReasonType.Deceased)]
        [DataRow(BreathingSpaceEndReasonType.InvalidInformation)]
        [DataRow(BreathingSpaceEndReasonType.NoLongerEligible)]
        [DataRow(BreathingSpaceEndReasonType.NotCompliedWithObligations)]
        [DataRow(BreathingSpaceEndReasonType.StoppedTreatment)]
        [DataRow(BreathingSpaceEndReasonType.UnableToContactClient)]
        [DataRow(BreathingSpaceEndReasonType.UnableToReachPointOfContact)]
        public void SubmitOptions_Can_Have_All_Breathing_Space_End_Reason_Values(BreathingSpaceEndReasonType reason)
        {
            // Arrange
            Sut.SubmitOption = reason;
            ValidationContext.MemberName = nameof(Sut.SubmitOption);

            // Act
            var result = Validator.TryValidateProperty(Sut.SubmitOption, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void IsPartOfThirtyDayReview_Can_Have_Value(bool? isPartOfThirtyDayReview)
        {
            // Arrange
            Sut.IsPartOfThirtyDayReview = isPartOfThirtyDayReview;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReview);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReview, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }


        [TestMethod]
        [DataRow(null)]
        public void IsPartOfThirtyDayReview_Can_Be_Null(bool? isPartOfThirtyDayReview)
        {
            // Arrange
            Sut.IsPartOfThirtyDayReview = isPartOfThirtyDayReview;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReview);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReview, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(BreathingSpaceEndReasonNoLongerEligibleReasonType.DebtReliefOrder)]
        [DataRow(BreathingSpaceEndReasonNoLongerEligibleReasonType.HasHadAnotherBreathingSpaceWithinTwelveMonths)]
        [DataRow(BreathingSpaceEndReasonNoLongerEligibleReasonType.InterimOrderOrIndividualVoluntaryArrangement)]
        [DataRow(BreathingSpaceEndReasonNoLongerEligibleReasonType.NotInEnglandOrWales)]
        [DataRow(BreathingSpaceEndReasonNoLongerEligibleReasonType.UndischargedBankrupt)]
        public void NoLongerEligibleReason_Can_Have_All_Breathing_Space_End_Reason_Values(BreathingSpaceEndReasonNoLongerEligibleReasonType? reason)
        {
            // Arrange
            Sut.NoLongerEligibleReason = reason;
            ValidationContext.MemberName = nameof(Sut.NoLongerEligibleReason);

            // Act
            var result = Validator.TryValidateProperty(Sut.NoLongerEligibleReason, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(null)]
        public void NoLongerEligibleReason_Can_Be_Null(BreathingSpaceEndReasonNoLongerEligibleReasonType? reason)
        {
            // Arrange
            Sut.NoLongerEligibleReason = reason;
            ValidationContext.MemberName = nameof(Sut.NoLongerEligibleReason);

            // Act
            var result = Validator.TryValidateProperty(Sut.NoLongerEligibleReason, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("ReasonMessage")]
        [DataRow("")]
        [DataRow(null)]
        public void ReasonMessage_Can_Have_Value_Or_Be_Null(string message)
        {
            // Arrange
            Sut.ReasonMessage = message;
            ValidationContext.MemberName = nameof(Sut.ReasonMessage);

            // Act
            var result = Validator.TryValidateProperty(Sut.ReasonMessage, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }
    }
}
