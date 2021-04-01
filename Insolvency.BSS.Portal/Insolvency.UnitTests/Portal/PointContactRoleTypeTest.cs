using Insolvency.Common;
using Insolvency.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.UnitTests.Portal
{
    [TestClass]
    public class PointContactRoleTypeTest
    {
        [TestMethod]
        public void CareCoordinator_Display_Name_Should_Match()
        {
            // Arrange
            PointContactRoleType inputValue = PointContactRoleType.CareCoordinator;

            // Act
            var result = inputValue.GetDisplayName();

            // Arrange
            Assert.AreEqual("Care co-ordinator", result);
        }

        [TestMethod]
        public void MentalHealthProgessional_Display_Name_Should_Match()
        {
            // Arrange
            PointContactRoleType inputValue = PointContactRoleType.MentalHealthProfessional;

            // Act
            var result = inputValue.GetDisplayName();

            // Arrange
            Assert.AreEqual("Approved Mental Health Professional", result);
        }

        [TestMethod]
        public void MentalHealthNurse_Display_Name_Should_Match()
        {
            // Arrange
            PointContactRoleType inputValue = PointContactRoleType.MentalHealthNurse;

            // Act
            var result = inputValue.GetDisplayName();

            // Arrange
            Assert.AreEqual("Mental health nurse", result);
        }

        [TestMethod]
        public void CareCoordinator_Int_Value_Should_Match()
        {
            // Arrange
            PointContactRoleType inputValue = PointContactRoleType.CareCoordinator;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void MentalHealthProgessional_Int_Value_Should_Match()
        {
            // Arrange
            PointContactRoleType inputValue = PointContactRoleType.MentalHealthProfessional;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void MentalHealthNurse_Int_Value_Should_Match()
        {
            // Arrange
            PointContactRoleType inputValue = PointContactRoleType.MentalHealthNurse;

            // Act
            var result = (int)inputValue;

            // Arrange
            Assert.AreEqual(2, result);
        }
    }
}
