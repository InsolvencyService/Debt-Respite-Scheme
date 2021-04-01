using System;
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
    public class DebtorNominatedContactViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorNominatedContactViewModel Sut { get; set; }
        public ValidationContext ValidationContext { get; }
        public Random Random { get; }

        public DebtorNominatedContactViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Build<DebtorNominatedContactViewModel>().WithAutoProperties().Without(x => x.TelephoneNumber).Create();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
            Random = new Random();
        }

        [TestMethod]
        public void PointContactRole_Cant_Be_Null()
        {
            // Arrange
            Sut.PointContactRole = null;
            ValidationContext.MemberName = nameof(Sut.PointContactRole);

            // Act
            var result = Validator.TryValidateProperty(Sut.PointContactRole, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PointContactRole_Cant_Be_Empty_String()
        {
            // Arrange
            Sut.PointContactRole = string.Empty;
            ValidationContext.MemberName = nameof(Sut.PointContactRole);

            // Act
            var result = Validator.TryValidateProperty(Sut.PointContactRole, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PointContactRole_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.PointContactRole = (Fixture.Create<PointContactRoleType>()).ToString();
            ValidationContext.MemberName = nameof(Sut.PointContactRole);

            // Act
            var result = Validator.TryValidateProperty(Sut.PointContactRole, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void FullName_Is_Required(string n)
        {
            // Arrange
            Sut.FullName = n;
            ValidationContext.MemberName = nameof(Sut.FullName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FullName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FullName_Is_Valid_If_It_Has_Value()
        {
            // Arrange
            Sut.FullName = Fixture.Create<string>();
            ValidationContext.MemberName = nameof(Sut.FullName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FullName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void FullName_Is_Valid_Up_To_100_Chars(int n)
        {
            // Arrange
            var fullName = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.FullName = fullName;
            ValidationContext.MemberName = nameof(Sut.FullName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FullName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(101)]
        [DataRow(200)]
        public void FullName_Is_Invalid_Over_100_Chars(int n)
        {
            // Arrange
            var fullName = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.FullName = fullName;
            ValidationContext.MemberName = nameof(Sut.FullName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FullName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("+4408081570192")]
        [DataRow("+4472478935468")]
        public void TelephoneNumber_Is_Valid(string n)
        {
            // Arrange
            Sut.TelephoneNumber = n;
            ValidationContext.MemberName = nameof(Sut.TelephoneNumber);

            // Act
            var result = Validator.TryValidateProperty(Sut.TelephoneNumber, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("+44951+")]
        [DataRow("onetwothree")]
        [DataRow("93764579345934")]
        [DataRow("+&3£4$5%6^7&")]
        public void TelephoneNumber_Is_Invalid(string n)
        {
            // Arrange
            Sut.TelephoneNumber = n;
            ValidationContext.MemberName = nameof(Sut.TelephoneNumber);

            // Act
            var result = Validator.TryValidateProperty(Sut.TelephoneNumber, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EmailAddress_Is_Invalid_If_ContactConfirmationMethod_Is_Email_And_EmailAddress_Is_Null()
        {
            // Arrange
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.EmailAddress = null;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(257)]
        [DataRow(500)]
        public void EmailAddress_Is_Invalid_If_Length_Is_Over256Chars_And_ContactConfirmationMethod_Is_Email(int n)
        {
            var email = $"{RandomString(n)}{Fixture.Create<MailAddress>().Address}";
            // Arrange
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.EmailAddress = email;
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
        public void EmailAddress_Is_Invalid_If_ContactConfirmationMethod_Is_Email_And_EmailAddress_Is_Invalid(string email)
        {
            // Arrange
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EmailAddress_Is_Valid_If_ContactConfirmationMethod_Is_Email_And_EmailAddress_Is_Valid()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.EmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.EmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }        

        [TestMethod]
        public void ConfirmEmailAddress_Is_Invalid_If_ContactConfirmationMethod_Is_Email_And_ConfirmEmailAddress_Is_Null()
        {
            // Arrange
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.ConfirmEmailAddress = null;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(257)]
        [DataRow(500)]
        public void ConfirmEmailAddress_Is_Invalid_If_Length_Is_Over256Char(int n)
        {
            var email = $"{RandomString(n)}{Fixture.Create<MailAddress>().Address}";
            // Arrange
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.ConfirmEmailAddress = email;
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
        public void ConfirmEmailAddress_Is_Invalid_If_It_Is_Invalid_Email(string email)
        {
            // Arrange
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.EmailAddress = email;
            Sut.ConfirmEmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ConfirmEmailAddress_Is_Valid_If_EmailAddress_And_ConfirmEmailAddress_Are_Identicial()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.EmailAddress = email;
            Sut.ConfirmEmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ConfirmEmailAddress_Is_Invalid_If_Not_Identical_To_EmailAddress()
        {
            // Arrange
            var email = Fixture.Create<MailAddress>().Address;
            var confirmEmail = Fixture.Create<MailAddress>().Address;
            Sut.ContactConfirmationMethod = $"{PointContactConfirmationMethod.Email}";
            Sut.ConfirmEmailAddress = confirmEmail;
            Sut.EmailAddress = email;
            ValidationContext.MemberName = nameof(Sut.ConfirmEmailAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.ConfirmEmailAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ContactConfirmationMethod_Is_Required(string n)
        {
            // Arrange
            Sut.ContactConfirmationMethod = n;
            ValidationContext.MemberName = nameof(Sut.ContactConfirmationMethod);

            // Act
            var result = Validator.TryValidateProperty(Sut.ContactConfirmationMethod, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("1")]
        [DataRow("2")]
        public void ContactConfirmationMethod_Is_Valid_If_It_Has_Value(string n)
        {
            // Arrange
            Sut.ContactConfirmationMethod = n;
            ValidationContext.MemberName = nameof(Sut.ContactConfirmationMethod);

            // Act
            var result = Validator.TryValidateProperty(Sut.ContactConfirmationMethod, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
