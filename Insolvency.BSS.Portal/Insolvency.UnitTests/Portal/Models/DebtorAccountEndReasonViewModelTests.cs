using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorAccountEndReasonViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorAccountEndReasonViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorAccountEndReasonViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorAccountEndReasonViewModel>();
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

        public void SubmitOption_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.SubmitOption = Fixture.Create<BreathingSpaceEndReasonType>();
            Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation = Fixture.Create<bool>();
            Sut.IsPartOfThirtyDayReviewStoppedTreament = Fixture.Create<bool>();
            Sut.IsPartOfThirtyDayReviewUnableToContactClient = Fixture.Create<bool>();
            Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact = Fixture.Create<bool>();
            Sut.IsPartOfThirtyDayReviewUsingDebtManagement = Fixture.Create<bool>();
            ValidationContext.MemberName = nameof(Sut.SubmitOption);

            // Act
            var result = Validator.TryValidateProperty(Sut.SubmitOption, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_StoppedTreatment_IsPartOfThirtyDayReviewStoppedTreament_Is_Invalid_If_Its_Null()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.StoppedTreatment;
            Sut.IsPartOfThirtyDayReviewStoppedTreament = null;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewStoppedTreament);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewStoppedTreament, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void For_StoppedTreatment_IsPartOfThirtyDayReviewStoppedTreament_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.StoppedTreatment;
            Sut.IsPartOfThirtyDayReviewStoppedTreament = Fixture.Create<bool>();
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewStoppedTreament);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewStoppedTreament, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_UnableToReachPointOfContact_IsPartOfThirtyDayReviewUnableToReachPointOfContact_Is_Invalid_If_Its_Null()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.UnableToReachPointOfContact;
            Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact = null;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void For_UnableToReachPointOfContact_IsPartOfThirtyDayReviewUnableToReachPointOfContact_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.UnableToReachPointOfContact;
            Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact = Fixture.Create<bool>();
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUnableToReachPointOfContact, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_NotCompliedWithObligations_IsPartOfThirtyDayReviewNotCompliedWithObligation_Is_Invalid_If_Its_Null()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.NotCompliedWithObligations;
            Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation = null;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void For_NotCompliedWithObligations_IsPartOfThirtyDayReviewNotCompliedWithObligation_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.NotCompliedWithObligations;
            Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation = Fixture.Create<bool>();
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewNotCompliedWithObligation, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_UnableToContactClient_IsPartOfThirtyDayReviewUnableToContactClient_Is_Invalid_If_Null()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.UnableToContactClient;
            Sut.IsPartOfThirtyDayReviewUnableToContactClient = null;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUnableToContactClient);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUnableToContactClient, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void For_UnableToContactClient_IsPartOfThirtyDayReviewUnableToContactClient_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.UnableToContactClient;
            Sut.IsPartOfThirtyDayReviewUnableToContactClient = Fixture.Create<bool>();
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUnableToContactClient);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUnableToContactClient, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_DebtManagementSolution_IsPartOfThirtyDayReviewUsingDebtManagement_Is_Invalid_If_Null_And_Is_Standard_Moratorium()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.DebtManagementSolution;
            Sut.IsPartOfThirtyDayReviewUsingDebtManagement = null;
            Sut.IsInMentalHealthMoratorium = false;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUsingDebtManagement);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUsingDebtManagement, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void For_DebtManagementSolution_IsPartOfThirtyDayReviewUsingDebtManagement_Is_Valid_If_It_Has_Value_And_Is_Standard_Moratorium()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.DebtManagementSolution;
            Sut.IsPartOfThirtyDayReviewUsingDebtManagement = Fixture.Create<bool>();
            Sut.IsInMentalHealthMoratorium = false;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUsingDebtManagement);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUsingDebtManagement, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_DebtManagementSolution_IsPartOfThirtyDayReviewUsingDebtManagement_Is_Valid_If_Null_And_Is_MentalHealth_Moratorium()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.DebtManagementSolution;
            Sut.IsPartOfThirtyDayReviewUsingDebtManagement = null;
            Sut.IsInMentalHealthMoratorium = true;
            ValidationContext.MemberName = nameof(Sut.IsPartOfThirtyDayReviewUsingDebtManagement);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsPartOfThirtyDayReviewUsingDebtManagement, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void For_NoLongerEligible_NoLongerEligibleReason_Is_Invalid_If_Its_Null()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.NoLongerEligible;
            Sut.NoLongerEligibleReason = null;
            ValidationContext.MemberName = nameof(Sut.NoLongerEligibleReason);

            // Act
            var result = Validator.TryValidateProperty(Sut.NoLongerEligibleReason, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void For_NoLongerEligible_NoLongerEligibleReason_Is_Valid_If_Reason_Is_It_Has_Value()
        {
            // Arrange
            Sut.SubmitOption = BreathingSpaceEndReasonType.NoLongerEligible;
            Sut.NoLongerEligibleReason = Fixture.Create<BreathingSpaceEndReasonNoLongerEligibleReasonType>();
            ValidationContext.MemberName = nameof(Sut.NoLongerEligibleReason);

            // Act
            var result = Validator.TryValidateProperty(Sut.NoLongerEligibleReason, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }

    }
}
