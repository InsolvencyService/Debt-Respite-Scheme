using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class CreditorAddAdHocDebtViewModelPropertyTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public CreditorAddAdHocDebtViewModel Sut { get; }
        public ValidationContext ValidationContext { get; }
        public Random Random { get; }

        public CreditorAddAdHocDebtViewModelPropertyTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<CreditorAddAdHocDebtViewModel>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
            Random = new Random();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void CreditorAddAdHocDebtViewModel_Reference_Is_Valid_Up_To_100_Chars(int n)
        {
            // Arrange
            var reference = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.Reference = reference;
            ValidationContext.MemberName = nameof(Sut.Reference);

            // Act
            var result = Validator.TryValidateProperty(Sut.Reference, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(101)]
        [DataRow(200)]
        public void CreditorAddAdHocDebtViewModel_Reference_Is_Invalid_Over_100_Chars(int n)
        {
            // Arrange
            var reference = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.Reference = reference;
            ValidationContext.MemberName = nameof(Sut.Reference);

            // Act
            var result = Validator.TryValidateProperty(Sut.Reference, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void CreditorAddAdHocDebtViewModel_SelectedDebtTypeName_Is_Valid_Up_To_100_Chars(int n)
        {
            // Arrange
            var debtType = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.SelectedDebtTypeName = debtType;
            ValidationContext.MemberName = nameof(Sut.SelectedDebtTypeName);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedDebtTypeName, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(101)]
        [DataRow(200)]
        public void CreditorAddAdHocDebtViewModel_SelectedDebtTypeName_Is_Invalid_Over_100_Chars(int n)
        {
            // Arrange
            var debtType = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.SelectedDebtTypeName = debtType;
            ValidationContext.MemberName = nameof(Sut.SelectedDebtTypeName);

            // Act
            var result = Validator.TryValidateProperty(Sut.SelectedDebtTypeName, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("12")]
        [DataRow("100")]
        [DataRow("1.1")]
        [DataRow("12.2")]
        [DataRow("101.3")]
        [DataRow("1.23")]
        [DataRow("12.55")]
        [DataRow("105.78")]
        public void CreditorAddAdHocDebtViewModel_DebtAmount_Is_A_Valid_2Decimal_Number(string debtAmount)
        {
            // Arrange
            Sut.DebtAmount = debtAmount;
            ValidationContext.MemberName = nameof(Sut.DebtAmount);

            // Act
            var result = Validator.TryValidateProperty(Sut.DebtAmount, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("asdasd")]
        [DataRow("123e4")]
        [DataRow("asdas123123asd")]
        [DataRow("123,45")]
        [DataRow("54:64")]
        public void CreditorAddAdHocDebtViewModel_DebtAmount_Is_NaN(string debtAmount)
        {
            // Arrange
            Sut.DebtAmount = debtAmount;
            ValidationContext.MemberName = nameof(Sut.DebtAmount);

            // Act
            var result = Validator.TryValidateProperty(Sut.DebtAmount, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void CreditorAddAdHocDebtViewModel_DebtAmount_Is_Valid_Up_To_100_Chars(int n)
        {
            // Arrange
            var debtAmount = GenerateNumberUpTo(n);
            Sut.DebtAmount = debtAmount;
            ValidationContext.MemberName = nameof(Sut.DebtAmount);

            // Act
            var result = Validator.TryValidateProperty(Sut.DebtAmount, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow(101)]
        [DataRow(200)]
        public void CreditorAddAdHocDebtViewModel_DebtAmount_Is_Invalid_Over_100_Chars(int n)
        {
            // Arrange
            var debtAmount = GenerateNumberUpTo(n);
            Sut.DebtAmount = debtAmount;
            ValidationContext.MemberName = nameof(Sut.DebtAmount);

            // Act
            var result = Validator.TryValidateProperty(Sut.DebtAmount, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        private string GenerateNumberUpTo(int n)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < n; i++)
                sb.Append(Random.Next(10).ToString());

            return sb.ToString();
        }
    }
}
