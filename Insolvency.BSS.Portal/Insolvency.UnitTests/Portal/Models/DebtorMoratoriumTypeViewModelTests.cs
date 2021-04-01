using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorMoratoriumTypeViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorBreathingSpaceTypeViewModel Sut { get; set; }
        public ValidationContext ValidationContext { get; }

        public DebtorMoratoriumTypeViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorBreathingSpaceTypeViewModel>();
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
            Sut.SubmitOption = Fixture.Create<BreathingSpaceType>().ToString();
            ValidationContext.MemberName = nameof(Sut.SubmitOption);

            // Act
            var result = Validator.TryValidateProperty(Sut.SubmitOption, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
