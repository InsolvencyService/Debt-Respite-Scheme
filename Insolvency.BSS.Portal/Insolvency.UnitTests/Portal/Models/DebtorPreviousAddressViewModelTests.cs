using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorPreviousAddressViewModelTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public DebtorPreviousAddressViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtorPreviousAddressViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorPreviousAddressViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        public void DebtorPreviousAddressViewModel_IsValid()
        {
            // Arrange & Act
            var result = Validator.TryValidateObject(Sut, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void DebtorPreviousAddressViewModel_SelectedAddressIsRequired(string address)
        {
            // Arrange
            Sut.SelectedAddress = address;
            ValidationContext.MemberName = nameof(Sut.SelectedAddress);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedAddress, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DebtorPreviousAddressViewModel_MonthFromIsRequired()
        {
            // Arrange
            Sut.MonthFrom = null;

            // Act
            var result = Validator.TryValidateObject(Sut, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveInDate);
        }
        [TestMethod]
        public void DebtorPreviousAddressViewModel_YearFromIsRequired()
        {
            // Arrange
            Sut.YearFrom = null;

            // Act
            var result = Validator.TryValidateObject(Sut, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveInDate);
        }

        [TestMethod]
        public void DebtorPreviousAddressViewModel_MonthToIsRequired()
        {
            // Arrange
            Sut.MonthTo = null;

            // Act
            var result = Validator.TryValidateObject(Sut, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveOutDate);
        }
        [TestMethod]
        public void DebtorPreviousAddressViewModel_YearToIsRequired()
        {
            // Arrange
            Sut.YearTo = null;

            // Act
            var result = Validator.TryValidateObject(Sut, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveOutDate);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(8)]
        [DataRow(12)]
        public void DebtorPreviousAddressViewModel_MonthFromFieldRange1To12IsValid(int? monthFrom)
        {
            // Arrange
            Sut.MonthFrom = monthFrom;
            ValidationContext.MemberName = nameof(Sut.MonthFrom);
            // Act
            var result = Validator.TryValidateProperty(Sut.MonthFrom, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(Sut.MoveInDate);
        }

        [TestMethod]
        [DataRow(13)]
        [DataRow(99)]
        [DataRow(0)]
        [DataRow(-1)]
        public void DebtorPreviousAddressViewModel_MonthFromFieldRange1To12IsNotValid(int? monthFrom)
        {
            // Arrange
            Sut.MonthFrom = monthFrom;
            ValidationContext.MemberName = nameof(Sut.MonthFrom);
            // Act
            var result = Validator.TryValidateProperty(Sut.MonthFrom, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveInDate);
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(9)]
        [DataRow(12)]
        public void DebtorPreviousAddressViewModel_MonthToFieldRange1To12IsValid(int? monthTo)
        {
            // Arrange
            Sut.MonthTo = monthTo;
            ValidationContext.MemberName = nameof(Sut.MonthTo);
            // Act
            var result = Validator.TryValidateProperty(Sut.MonthTo, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(Sut.MoveOutDate);
        }

        [TestMethod]
        [DataRow(13)]
        [DataRow(99)]
        [DataRow(0)]
        [DataRow(-1)]
        public void DebtorPreviousAddressViewModel_MonthToFieldRange1To12IsNotValid(int? monthTo)
        {
            // Arrange
            Sut.MonthTo = monthTo;
            ValidationContext.MemberName = nameof(Sut.MonthTo);
            // Act
            var result = Validator.TryValidateProperty(Sut.MonthTo, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveOutDate);
        }

        [TestMethod]
        [DataRow(1880)]
        [DataRow(2200)]
        [DataRow(1995)]
        public void DebtorPreviousAddressViewModel_YearFromFieldRange1880To2200IsValid(int? yearFrom)
        {
            // Arrange
            Sut.YearFrom = yearFrom;
            ValidationContext.MemberName = nameof(Sut.YearFrom);

            // Act
            var result = Validator.TryValidateProperty(Sut.YearFrom, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
            Assert.IsNotNull(Sut.MoveInDate);
        }

        [TestMethod]
        [DataRow(2201)]
        [DataRow(1879)]
        [DataRow(0)]
        public void DebtorPreviousAddressViewModel_YearFromFieldRange1880To2200IsNotValid(int? yearFrom)
        {
            // Arrange
            Sut.YearFrom = yearFrom;
            ValidationContext.MemberName = nameof(Sut.YearFrom);

            // Act
            var result = Validator.TryValidateProperty(Sut.YearFrom, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(1880)]
        [DataRow(2200)]
        [DataRow(1995)]
        public void DebtorPreviousAddressViewModel_YearToFieldRange1880To2200IsValid(int? yearTo)
        {
            // Arrange
            Sut.YearTo = yearTo;
            ValidationContext.MemberName = nameof(Sut.YearTo);

            // Act
            var result = Validator.TryValidateProperty(Sut.YearTo, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
            Assert.IsNotNull(Sut.MoveOutDate);
        }

        [TestMethod]
        [DataRow(2201)]
        [DataRow(1879)]
        [DataRow(0)]
        public void DebtorPreviousAddressViewModel_YearToFieldRange1880To2200IsNotValid(int? yearTo)
        {
            // Arrange
            Sut.YearTo = yearTo;
            ValidationContext.MemberName = nameof(Sut.YearTo);

            // Act
            var result = Validator.TryValidateProperty(Sut.YearTo, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(12, 1880)]
        [DataRow(1, 1995)]
        [DataRow(11,2200)]
        public void DebtorPreviousAddressViewModel_MoveInDateIsValid(int? monthFrom, int? yearFrom)
        {
            // Arrange
            Sut.MonthFrom = monthFrom;
            Sut.YearFrom = yearFrom;
            ValidationContext.MemberName = nameof(Sut.MoveInDate);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoveInDate, ValidationContext, Messages);
            
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(Sut.MoveInDate);
        }

        [TestMethod]
        [DataRow(12, 1879)]
        [DataRow(1, 2201)]
        public void DebtorPreviousAddressViewModel_MoveInDateIsNotValid(int? monthFrom, int? yearFrom)
        {
            // Arrange
            Sut.MonthFrom = monthFrom;
            Sut.YearFrom = yearFrom;
            ValidationContext.MemberName = nameof(Sut.MoveInDate);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoveInDate, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(15, 1995)]
        [DataRow(1, 20)]
        [DataRow(null, null)]
        public void DebtorPreviousAddressViewModel_MoveInDateIsNull(int? monthFrom, int? yearFrom)
        {
            // Arrange
            Sut.MonthFrom = monthFrom;
            Sut.YearFrom = yearFrom;
            ValidationContext.MemberName = nameof(Sut.MoveInDate);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoveInDate, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveInDate);
        }

        [TestMethod]
        [DataRow(12, 1880)]
        [DataRow(1, 1995)]
        [DataRow(11, 2200)]
        public void DebtorPreviousAddressViewModel_MoveOutDateIsValid(int? monthTo, int? yearTo)
        {
            // Arrange
            Sut.MonthTo = monthTo;
            Sut.YearTo = yearTo;
            ValidationContext.MemberName = nameof(Sut.MoveOutDate);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoveOutDate, ValidationContext, Messages);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(Sut.MoveOutDate);
        }

        [TestMethod]
        [DataRow(12, 1879)]
        [DataRow(1, 2201)]
        public void DebtorPreviousAddressViewModel_MoveOutDateIsNotValid(int? monthTo, int? yearTo)
        {
            // Arrange
            Sut.MonthTo = monthTo;
            Sut.YearTo = yearTo;
            ValidationContext.MemberName = nameof(Sut.MoveOutDate);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoveOutDate, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(15, 1995)]
        [DataRow(1, 20)]
        [DataRow(null, null)]
        public void DebtorPreviousAddressViewModel_MoveOutDateIsNull(int? monthTo, int? yearTo)
        {
            // Arrange
            Sut.MonthTo = monthTo;
            Sut.YearTo = yearTo;
            ValidationContext.MemberName = nameof(Sut.MoveOutDate);

            // Act
            var result = Validator.TryValidateProperty(Sut.MoveOutDate, ValidationContext, Messages);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(Sut.MoveOutDate);
        }


    }
}
