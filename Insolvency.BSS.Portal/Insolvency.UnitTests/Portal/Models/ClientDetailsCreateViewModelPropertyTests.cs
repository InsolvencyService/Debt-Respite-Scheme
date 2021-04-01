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
    public class ClientDetailsCreateViewModelPropertyTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public ClientDetailsCreateViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }
        public Random Random { get; }

        public ClientDetailsCreateViewModelPropertyTests()
        {
            this.Fixture = new Fixture();
            this.Sut = Fixture.Create<ClientDetailsCreateViewModel>();
            this.Messages = new List<ValidationResult>();
            this.ValidationContext = new ValidationContext(this.Sut);
            this.Random = new Random();
        }

        [TestMethod]
        public void ClientDetailsCreateViewModel_FirstNameIsValid()
        {
            // Arrange
            ValidationContext.MemberName = nameof(Sut.FirstName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FirstName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void ClientDetailsCreateViewModel_FirstNameIsRequired(string name)
        {
            // Arrange
            Sut.FirstName = name;
            ValidationContext.MemberName = nameof(Sut.FirstName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FirstName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ClientDetailsCreateViewModel_FirstNameHasMaxLength100()
        {
            // Arrange
            var name = new String(Fixture.CreateMany<char>(101).ToArray());
            Sut.FirstName = name;
            ValidationContext.MemberName = nameof(Sut.FirstName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FirstName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ClientDetailsCreateViewModel_FirstNameHasMaxLength100_2()
        {
            // Arrange
            var name = new String(Fixture.CreateMany<char>(100).ToArray());
            Sut.FirstName = name;
            ValidationContext.MemberName = nameof(Sut.FirstName);

            // Act
            var result = Validator.TryValidateProperty(Sut.FirstName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Valid_If_BirthYear_And_BirthMonth_And_BirthDay_Is_Within_Range()
        {
            // Arrange
            var today = DateTime.Now;
            var over16Year = today.Year - 17;
            Sut.BirthDay = Random.Next(1, 31);
            Sut.BirthMonth = Random.Next(1, 12);
            Sut.BirthYear = Random.Next(1880, over16Year);
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Invalid_If_BirthYear_And_BirthMonth_And_BirthDay_Is_Null()
        {
            // Arrange
            Sut.BirthDay = null;
            Sut.BirthMonth = null;
            Sut.BirthYear = null;
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Invalid_If_User_Enters_01_01_0001()
        {
            // Arrange
            Sut.BirthDay = 1;
            Sut.BirthMonth = 1;
            Sut.BirthYear = 1;
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Valid_If_16()
        {
            // Arrange
            var today = DateTime.Now;
            var birthYear = today.Year - 16;
            Sut.BirthDay = today.Day;
            Sut.BirthMonth = today.Month;
            Sut.BirthYear = birthYear;
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Valid_If_Over16()
        {
            // Arrange
            var today = DateTime.Now;
            var birthYear = today.Year - 17;
            Sut.BirthDay = today.Day;
            Sut.BirthMonth = today.Month;
            Sut.BirthYear = birthYear;
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Invalid_If_16_This_Year_But_Birthday_Hasnt_Happened_Yet()
        {
            // Arrange
            var birthday = DateTime.Now.AddYears(-16).AddDays(1);
            var birthDay = birthday.Day;
            var birthMonth = birthday.Month;
            var birthYear = birthday.Year;

            Sut.BirthDay = birthDay;
            Sut.BirthMonth = birthMonth;
            Sut.BirthYear = birthYear;
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Invalid_If_BirthYear_Is_Negative()
        {
            // Arrange
            Sut.BirthDay = Random.Next(1, 31);
            Sut.BirthMonth = Random.Next(1, 12);
            Sut.BirthYear = Random.Next(-2200, -1880);
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Invalid_If_BirthMonth_Is_Negative()
        {
            // Arrange
            Sut.BirthDay = Random.Next(1, 31);
            Sut.BirthMonth = Random.Next(-12, -1);
            Sut.BirthYear = Random.Next(1880, 2200);
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidDateOfBirth_Is_Invalid_If_BirthDay_Is_Negative()
        {
            // Arrange
            Sut.BirthDay = Random.Next(-31, -1);
            Sut.BirthMonth = Random.Next(1, 12);
            Sut.BirthYear = Random.Next(1880, 2200);
            ValidationContext.MemberName = nameof(Sut.IsValidDateOfBirth);

            // Act
            var result = Validator.TryValidateProperty(Sut.IsValidDateOfBirth, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }
    }
}
