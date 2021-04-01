using System;
using AutoFixture;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal.Models
{
    [TestClass]
    public class DebtorPersonalDetailViewModelTests
    {
        public Fixture Fixture { get; }
        public DebtorPersonalDetailViewModel Sut { get; }

        public DebtorPersonalDetailViewModelTests()
        {
            Fixture = new Fixture();
            Sut = Fixture.Create<DebtorPersonalDetailViewModel>();
        }

        [TestMethod]
        public void MoratoriumCurrentDay_Returns_Zero_When_Start_Is_In_The_Future()
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now + new TimeSpan(5, 0, 0, 0);
            var result = Sut.MoratoriumCurrentDay();

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void MoratoriumCurrentDay_Returns_One_When_Start_Is_Today()
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now;
            var result = Sut.MoratoriumCurrentDay();

            // Assert
            Assert.AreEqual(1, result);
        }


        [DataRow(2, 3)]
        [DataRow(5, 6)]
        [DataRow(10, 11)]
        [DataRow(23, 24)]
        [TestMethod]
        public void MoratoriumCurrentDay_Returns_Day_Difference(int daysBehind, int expected)
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now - new TimeSpan(daysBehind, 0, 0, 0);
            var result = Sut.MoratoriumCurrentDay();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [DataRow(2)]
        [DataRow(10)]
        [DataRow(19)]
        [TestMethod]
        public void EligibleForReview_Return_True_When_Current_Day_Lt_TwentyOne(int daysBehind)
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now - new TimeSpan(daysBehind, 0, 0, 0);
            var result = Sut.EligibleForReview;

            // Assert
            Assert.IsTrue(result);
        }

        [DataRow(20)]
        [DataRow(21)]
        [DataRow(50)]
        [TestMethod]
        public void EligibleForReview_Return_False_When_Current_Day_Gt_Or_Eq_TwentyOne(int daysBehind)
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now - new TimeSpan(daysBehind, 0, 0, 0);
            var result = Sut.EligibleForReview;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DisplayBreathingSpaceDates_Should_Return_False_When_Star_Date_Is_Null()
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = null;
            var result = Sut.DisplayBreathingSpaceDates;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DisplayBreathingSpaceDates_Should_Return_True_When_Star_Date_Has_Value()
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now;
            var result = Sut.DisplayBreathingSpaceDates;

            // Assert
            Assert.IsTrue(result);
        }

        [DataRow(20)]
        [DataRow(21)]
        [DataRow(50)]
        [TestMethod]
        public void MoratoriumLength_Should_Return_Day_Count_Difference(int days)
        {
            // Arrange & Act
            Sut.ActiveMoratoriumStartDate = DateTimeOffset.Now;
            Sut.ActiveMoratoriumEndDate = DateTimeOffset.Now + new TimeSpan(days, 0, 0, 0);
            var result = Sut.MoratoriumLength;

            // Assert
            Assert.AreEqual(days, result);
        }
    }
}
