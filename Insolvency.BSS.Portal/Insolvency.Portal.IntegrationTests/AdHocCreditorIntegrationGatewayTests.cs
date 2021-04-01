using System;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Portal.Gateways;
using Insolvency.Portal.Models.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Portal.IntegrationTests
{
    [TestClass]
    public class AdHocCreditorIntegrationGatewayTests : IntegrationGatewayBase
    {
        public static Guid AdHocCreditorId { get; set; }

        [ClassInitialize]
        public static async Task ClassInitAsync(TestContext context)
        {
            await InitialiseDebtorAsync();
            await InitialiseAdHocCreditorAsync();
        }

        [TestInitialize]
        public override void TestInit()
        {
            CheckIfAdHocCreditorWasCreated();
            base.TestInit();
        }

        public static async Task InitialiseAdHocCreditorAsync()
        {
            // Arrange
            var sut = new IntegrationGateway(Main.ApiClient);
            var model = Main.Fixture.Create<string>();
            Main.SetMoneyAdviserCreditorApiCient();

            // Act & Asssert
            var result = await sut.CreateAdHocCreditor(model);
            AdHocCreditorId = result;
        }
        public void CheckIfAdHocCreditorWasCreated()
        {
            if (AdHocCreditorId == default(Guid))
            {
                Assert.Fail("AdHocCreditorId is not present, InitialiseAdHocCreditor failed");
            }
        }

        [TestMethod]
        public async Task CreateDebt_Should_Return_New_Id()
        {
            // Arrange
            var model = Main.Fixture.Build<DebtViewModel>()
                .WithAutoProperties()
                .Without(x => x.CreditorId)
                .Without(x => x.MoratoriumId)
                .Without(x => x.SelectedDebtTypeId)
                .Without(x => x.NINO)
                .Create();
            model.CreditorId = AdHocCreditorId;
            model.MoratoriumId = MoratoriumId;

            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            var result = await Sut.CreatDebtAsync(model);

            // Assert
            Assert.AreNotEqual(Guid.Empty, result);
        }

        [TestMethod]
        public async Task AdHocCreditor_Should_Not_Have_DebtTypes()
        {
            // Arrange
            var model = AdHocCreditorId.ToString();
            Main.SetMoneyAdviserCreditorApiCient();

            // Act
            var result = await Sut.GetGenericCreditorByIdAsync(model);

            // Assert
            Assert.IsNull(result.DebtTypes);
        }
    }
}
