using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class SearchParameterViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public SearchParameterViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public SearchParameterViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<SearchParameterViewModel>();
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
        public void When_Reference_IsNotNullOrEmpty_Then_Filter_Message_Returns_Correct_Value()
        {
            // Arrange
            var reference = Fixture.Create<string>();
            var sut = new SearchParameterViewModel(reference, null, null);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(result.Contains(reference));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public void When_Reference_IsNullOrEmpty_Then_Filter_Message_Returns_Correct_Value(string reference)
        {
            // Arrange
            var sut = new SearchParameterViewModel(reference, null, null);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(String.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void When_Surname_IsNotNullOrEmpty_Then_Filter_Message_Returns_Correct_Value()
        {
            // Arrange
            var surname = Fixture.Create<string>();
            var sut = new SearchParameterViewModel(null, surname, null);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(result.Contains(surname));
        }


        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void When_Surname_IsNullOrEmpty_Then_Filter_Message_Returns_Correct_Value(string surname)
        {
            // Arrange
            var sut = new SearchParameterViewModel(null, surname, null);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(String.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void When_DateOfBirth_IsNotNullOrEmpty_Then_Filter_Message_Returns_Correct_Value()
        {
            // Arrange
            var dateOfBirth = Fixture.Create<DateTime>().ToString();
            var sut = new SearchParameterViewModel(null, null, dateOfBirth);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(result.Contains(dateOfBirth));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void When_DateOfBirth_IsNullOrEmpty_Then_Filter_Message_Returns_Correct_Value(string dateOfBirth)
        {
            // Arrange
            var sut = new SearchParameterViewModel(null, null, dateOfBirth);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(String.IsNullOrEmpty(result));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void When_All_Filters_AreNullOrEmpty_Then_Filter_Message_Returns_Correct_Value(string filter)
        {
            // Arrange
            var sut = new SearchParameterViewModel(filter, filter, filter);

            // Act
            var result = sut.FilterMessage;

            // Assert     
            Assert.IsTrue(String.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void When_Reference_And_Surname_AreNotNullOrEmpty_Then_Filter_Message_Returns_Correct_Value()
        {
            // Arrange
            var reference = Fixture.Create<string>();
            var surname = Fixture.Create<string>();
            var sut = new SearchParameterViewModel(reference, surname, null);

            // Act
            var result = sut.FilterMessage;

            // Assert
            Assert.IsTrue(result.Contains(reference));
            Assert.IsTrue(result.Contains(surname));
        }

        [TestMethod]
        public void When_Reference_And_DateOfBirth_AreNotNullOrEmpty_Then_Filter_Message_Returns_Correct_Value()
        {
            // Arrange
            var reference = Fixture.Create<string>();
            var dateOfBirth = Fixture.Create<string>();
            var sut = new SearchParameterViewModel(reference, null, dateOfBirth);

            // Act
            var result = sut.FilterMessage;

            // Assert
            Assert.IsTrue(result.Contains(reference));
            Assert.IsTrue(result.Contains(dateOfBirth));
        }

        [TestMethod]

        public void When_Surname_And_DateOfBirth_AreNotNullOrEmpty_Then_Filter_Message_Returns_Correct_Value()
        {
            // Arrange
            var surname = Fixture.Create<string>();
            var dateOfBirth = Fixture.Create<string>();
            var sut = new SearchParameterViewModel(null, surname, dateOfBirth);

            // Act
            var result = sut.FilterMessage;

            // Assert
            Assert.IsTrue(result.Contains(surname));
            Assert.IsTrue(result.Contains(dateOfBirth));
        }
    }
}
