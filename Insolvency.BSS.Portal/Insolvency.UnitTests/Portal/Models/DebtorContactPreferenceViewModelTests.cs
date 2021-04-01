using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using AutoFixture;
using Insolvency.Common.Enums;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorContactPreferenceViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorContactPreferenceViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorContactPreferenceViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorContactPreferenceViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        public void DebtorPreference_Cant_Be_Null()
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
        public void DebtorEmailAddress_Shouldnt_Validate_Required_If_Preference_Is_Not_Email()
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.None;
            Sut.EmailAddress = null;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DebtorEmailAddress_Should_Validate_Required_If_Preference_Is_Email()
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.EmailAddress = null;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DebtorEmailAddress_Should_Validate_Length_Is_Over_If_Preference_Is_Email()
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.EmailAddress = new string(Fixture.CreateMany<char>(300).ToArray());
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("Test")]
        [DataRow("test.com")]
        [DataRow("@test.com")]
        [DataRow("test+test.com")]
        public void DebtorEmailAddress_Should_Validate_Invalid_Email_If_Preference_Is_Email(string email)
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DebtorEmailAddress_Should_Validate_Valid_Email_If_Preference_Is_Email()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DebtorConfirmEmailAddress_Shouldnt_Validate_Required_If_Preference_Is_Not_Email()
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.None;
            Sut.EmailAddress = null;
            Sut.ConfirmEmailAddress = null;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DebtorConfirmEmailAddress_Should_Validate_Required_If_Preference_Is_Email()
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.ConfirmEmailAddress = null;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DebtorConfirmEmailAddress_Should_Validate_Length_Is_Over_If_Preference_Is_Email()
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.ConfirmEmailAddress = new string(Fixture.CreateMany<char>(300).ToArray());
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("Test")]
        [DataRow("test.com")]
        [DataRow("@test.com")]
        [DataRow("test+test.com")]
        public void DebtorConfirmEmailAddress_Should_Validate_Invalid_Email_If_Preference_Is_Email(string email)
        {
            // Arrange
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.ConfirmEmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DebtorConfirmEmailAddress_Should_Validate_Valid_Email_If_Preference_Is_Email()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.EmailAddress = email;
            Sut.ConfirmEmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DebtorConfirmEmailAddress_Should_Match_EmailAddress()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.ConfirmEmailAddress = email;
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DebtorConfirmEmailAddress_Should_Not_Match_EmailAddress()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            var confirmEmail = Fixture.Create<MailAddress>().Address;
            Sut.SubmitOption = ContactPreferenceType.Email;
            Sut.ConfirmEmailAddress = confirmEmail;
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }
    }
}
