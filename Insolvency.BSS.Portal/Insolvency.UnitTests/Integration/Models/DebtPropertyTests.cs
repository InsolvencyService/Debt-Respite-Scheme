using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using Insolvency.Integration.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Integration.Models
{
    [TestClass]
    public class DebtPropertyTests
    {
        public Fixture Fixture { get; }
        public List<ValidationResult> Messages { get; }
        public Debt Sut { get; }
        public ValidationContext ValidationContext { get; }

        public DebtPropertyTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<Debt>();
            Messages = new List<ValidationResult>();
            ValidationContext = new ValidationContext(Sut);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(50)]
        [DataRow(99)]
        [DataRow(100)]
        public void Debt_Reference_Is_Valid_Up_To_100_Chars(int n)
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
        public void Debt_Reference_Is_Invalid_Over_100_Chars(int n)
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
        [DataRow("CZ 61 55 46 D")]
        [DataRow("EM053409B")]
        [DataRow("RS 93 06 64 A")]
        [DataRow("JB639786C")]
        [DataRow("JM 10 95 27 B")]
        [DataRow("KK654518D")]
        [DataRow("WJ 93 51 77 C")]
        [DataRow("EK959767D")]
        [DataRow("AL 26 73 86 C")]
        [DataRow("EP623846C")]
        public void Debt_NIN_Matches_Regex(string nin)
        {
            // Arrange
            Sut.NINO = nin;
            ValidationContext.MemberName = nameof(Sut.NINO);

            // Act
            var result = Validator.TryValidateProperty(Sut.NINO, ValidationContext, Messages);

            // Assert     
            Assert.IsTrue(result);
        }

        [TestMethod]
        [DataRow("asdasdasd")]
        [DataRow("053409")]
        [DataRow("RS 93 06 64")]
        [DataRow("JB639786")]
        [DataRow("BG 10 95 27 B")]
        [DataRow("GB654518D")]
        [DataRow("NK 93 51 77 C")]
        [DataRow("KN959767D")]
        [DataRow("NT 26 73 86 C")]
        [DataRow("TN623846C")]
        [DataRow("ZZ623846C")]
        public void Debt_NIN_Doesnt_Match_Regex(string nin)
        {
            // Arrange
            Sut.NINO = nin;
            ValidationContext.MemberName = nameof(Sut.NINO);

            // Act
            var result = Validator.TryValidateProperty(Sut.NINO, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow(14)]
        [DataRow(100)]
        public void Debt_Reference_Is_Invalid_Over_13_Chars(int n)
        {
            // Arrange
            var nin = new String(Fixture.CreateMany<char>(n).ToArray());
            Sut.NINO = nin;
            ValidationContext.MemberName = nameof(Sut.NINO);

            // Act
            var result = Validator.TryValidateProperty(Sut.NINO, ValidationContext, Messages);

            // Assert     
            Assert.IsFalse(result);
        }
    }
}
