using System;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels.Creditor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models.Creditor
{
    [TestClass]
    public class CreditorDebtPartialViewModelTests
    {
        public Fixture Fixture { get; }

        public CreditorDebtPartialViewModelTests() => Fixture = new Fixture();

        [TestMethod]
        public void FormattedBreathingSpaceEndDate_Should_Format_EndDate_Correctly()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2020, 11, 10, 0, 0, 0, new TimeSpan());
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.BreathingSpaceEndDate, dateTime)
                .Create();

            // Act
            var result = model.FormattedBreathingSpaceEndDate;

            // Assert
            Assert.AreEqual(result, "10 November 2020");
        }

        [TestMethod]
        public void FormattedBreathingSpaceEndDate_Should_Return_Null_When_EndDate_Is_Null()
        {
            // Arrange
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.BreathingSpaceEndDate, (DateTimeOffset?)null)
                .Create();

            // Act
            var result = model.FormattedBreathingSpaceEndDate;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FormattedCreatedOn_Should_Format_CreatedOn_Correctly()
        {
            // Arrange
            var dateTime = new DateTimeOffset(2020, 11, 10, 0, 0, 0, new TimeSpan());
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.StartsOn, dateTime)
                .Create();

            // Act
            var result = model.FormattedStartsOn;

            // Assert
            Assert.AreEqual(result, "10 November 2020");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("            ")]
        [DataRow("0")]
        public void HasAmount_Returns_False_Correctly(string amount)
        {
            // Arrange            
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.Amount, amount)
                .Create();

            // Act
            var result = model.HasAmount;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("            ")]
        public void HasReference_Returns_False_Correctly(string amount)
        {
            // Arrange            
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.Reference, amount)
                .Create();

            // Act
            var result = model.HasReference;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("            ")]
        public void HasNINO_Returns_False_Correctly(string amount)
        {
            // Arrange            
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.NINO, amount)
                .Create();

            // Act
            var result = model.HasNINO;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("            ")]
        public void HasType_Returns_False_Correctly(string amount)
        {
            // Arrange            
            var model = Fixture
                .Build<CreditorDebtPartialViewModel>()
                .With(x => x.Type, amount)
                .Create();

            // Act
            var result = model.HasType;

            // Assert
            Assert.IsFalse(result);
        }
    }
}
