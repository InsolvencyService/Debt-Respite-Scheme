using System;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Integration.Gateways;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Integration.IntegrationTests
{
    [TestClass]
    public class AdHocCreditorDynamicsGatewayTests : DynamicsGatewayBase
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
            var sut = new CommonDynamicsGateway(Main.Client, Main.DynamicsOptions, new NullLogger<CommonDynamicsGateway>(), null, Main.AuditContext, Main.AuditService);
            var model = Main.Fixture.Create<string>();

            // Act & Asssert
            var result = await sut.CreateAdHocCreditor(model);
            AdHocCreditorId = result;
        }

        public void CheckIfAdHocCreditorWasCreated()
        {
            if (AdHocCreditorId == default)
            {
                Assert.Fail("AdHocCreditorId is not present, InitialiseAdHocCreditor failed");
            }
        }

        [TestMethod]
        public async Task AdHocCreditor_Should_Not_Have_DebtTypes()
        {
            // Arrange
            var model = AdHocCreditorId;

            // Act
            var result = await Sut_SharedSvc.GetGenericCreditorById(model);

            // Assert
            Assert.IsNull(result.DebtTypes);
        }

        [TestMethod]
        public async Task AdHocCreditor_AddDebt_Should_Return_New_Id()
        {
            // Arrange
            var model = Main.Fixture.Build<CreateDebtRequest>()
                .WithAutoProperties()
                .Without(x => x.CreditorId)
                .Without(x => x.MoratoriumId)
                .Without(x => x.NINO)
                .Without(x => x.DebtTypeId)
                .Create();
            model.CreditorId = AdHocCreditorId;
            model.MoratoriumId = MoratoriumId;

            // Act
            var result = await Sut_MoneyAdvSvc.CreateDebtAsync(model);

            // Assert
            Assert.AreNotEqual(default, result);
        }
    }
}
