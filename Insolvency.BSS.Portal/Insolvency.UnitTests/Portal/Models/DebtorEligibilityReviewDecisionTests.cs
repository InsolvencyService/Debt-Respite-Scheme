using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Common;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorEligibilityReviewDecisionTests
    {

        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorEligibilityReviewDecisionViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorEligibilityReviewDecisionTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorEligibilityReviewDecisionViewModel>();
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
        public void Model_Must_Have_No_Parameters_Ctor()
        {
            var creditorName = Fixture.Create<string>();
            var moneyAdviserNotes = Fixture.Create<string>();
            var reason = Fixture.Create<string>();
            var creditorNotes = Fixture.Create<string>();
            var creditorOrganisation = Fixture.Create<string>();
            var submissionDate = Fixture.Create<string>();
            var moneyAdviserName = Fixture.Create<string>();
            var moneyAdviserOrganisation = Fixture.Create<string>();
            var reviewDate = Fixture.Create<string>();
            var endBreathingSpace = Fixture.Create<bool?>();
            // Arrange & Act
            var sut = new DebtorEligibilityReviewDecisionViewModel
            {
                CreditorName = creditorName,
                MoneyAdviserNotes = moneyAdviserNotes,
                CreditorNotes = creditorNotes,
                EndBreathingSpace = endBreathingSpace,
                CreditorOrganisation = creditorOrganisation,
                MoneyAdviserName = moneyAdviserName,
                MoneyAdviserOrganisation = moneyAdviserOrganisation,
            };

            // Assert
            Assert.AreEqual(creditorName, sut.CreditorName);
            Assert.AreEqual(moneyAdviserNotes, sut.MoneyAdviserNotes);
            Assert.AreEqual(creditorNotes, sut.CreditorNotes);
            Assert.AreEqual(endBreathingSpace, sut.EndBreathingSpace);
            Assert.AreEqual(creditorOrganisation, sut.CreditorOrganisation);
            Assert.AreEqual(moneyAdviserName, sut.MoneyAdviserName);
            Assert.AreEqual(moneyAdviserOrganisation, sut.MoneyAdviserOrganisation);
        }

        [TestMethod]
        [DataRow(null)]
        public void EndBreathingSpace_Is_Required(bool? endBreathingSpace)
        {
            // Arrange
            Sut.EndBreathingSpace = endBreathingSpace;
            ValidationContext.MemberName = nameof(Sut.EndBreathingSpace);

            // Act
            var result = Validator.TryValidateProperty(Sut.EndBreathingSpace, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("      ")]
        public void MoneyAdviserNotes_Are_Required(string moneyAdviserNotes)
        {
            // Arrange
            Sut.MoneyAdviserNotes = moneyAdviserNotes;
            ValidationContext.MemberName = nameof(Sut.MoneyAdviserNotes);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoneyAdviserNotes, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("CreditorOrganisation")]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void When_CreditorName_Is_Set_ClientEligibilityReviewSubtitle_Returns_Correct_Value(string creditorOrganisation)
        {
            // Arrange
            Sut.CreditorOrganisation = creditorOrganisation;

            // Act
            var result = Sut.ClientEligibilityReviewSubtitle;

            // Assert     
            Assert.AreEqual($"{creditorOrganisation} asked for a review", result);
        }

        [TestMethod]
        [DataRow("CreditorName", "CreditorOrganization")]
        [DataRow("CreditorName", "CreditorOrganization")]
        [DataRow(null, "CreditorOrganization")]
        [DataRow("CreditorName", null)]
        [DataRow("CreditorName", null)]
        [DataRow(null, "CreditorOrganization")]
        [DataRow(null, null)]
        [DataRow(null, null)]
        [DataRow("", "")]
        [DataRow("   ", "   ")]
        public void When_CreditorName_And_CreditorOrganisation_And_SubmissionDate_Are_Set_RequestedBy_Returns_Correct_Value(string creditorName,
            string creditorOrganisation)
        {
            // Arrange
            var createdOn = Fixture.Create<DateTimeOffset>();
            Sut.CreditorName = creditorName;
            Sut.CreditorOrganisation = creditorOrganisation;
            Sut.CreatedOn = createdOn;

            // Act
            var result = Sut.RequestedBy;

            // Assert     
            Assert.AreEqual($"{creditorName}, {creditorOrganisation}, {createdOn.ToString(Constants.PrettyDateFormat)} at {createdOn.ToString("HH.mm")}{createdOn.ToString("tt").ToLower()}", result);
        }

        [TestMethod]
        public void When_EndBreathingSpace_Is_true_ConfirmationTitle_Returns_Correct_Value()
        {
            // Arrange
            Sut.EndBreathingSpace = true;

            // Act
            var result = Sut.ConfirmationTitle;

            // Assert     
            Assert.AreEqual("Confirm you are ending your client’s Breathing Space", result);
        }

        [TestMethod]
        public void When_EndBreathingSpace_Is_false_ConfirmationTitle_Returns_Correct_Value()
        {
            // Arrange
            Sut.EndBreathingSpace = false;

            // Act
            var result = Sut.ConfirmationTitle;

            // Assert     
            Assert.AreEqual("Confirm you are not ending your client’s Breathing Space", result);
        }

        [TestMethod]
        public void When_EndBreathingSpace_Is_true_ConfirmationButtonText_Returns_Correct_Value()
        {
            // Arrange
            Sut.EndBreathingSpace = true;

            // Act
            var result = Sut.ConfirmationButtonText;

            // Assert     
            Assert.AreEqual("Confirm and end Breathing Space", result);
        }

        [TestMethod]
        public void When_EndBreathingSpace_Is_false_ConfirmationButtonText_Returns_Correct_Value()
        {
            // Arrange
            Sut.EndBreathingSpace = false;

            // Act
            var result = Sut.ConfirmationButtonText;

            // Assert     
            Assert.AreEqual("Confirm and continue", result);
        }
    }
}
