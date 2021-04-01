using System;
using System.Linq;
using System.Threading.Tasks;
using Insolvency.Integration.Gateways;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Integration.IntegrationTests
{
    [TestClass]
    public class CmpCreditorDynamicsGatewayTests : DynamicsGatewayBase
    {
        public static Guid CmpCreditorId { get; set; }

        [ClassInitialize]
        public static async Task ClassInitAsync(TestContext context)
        {
            await InitialiseDebtorAsync();
            await InitialiseCmpCreditorAsync();
        }

        [TestInitialize]
        public override void TestInit()
        {
            CheckIfCmpCreditorWasFound();
            base.TestInit();
        }

        public static async Task InitialiseCmpCreditorAsync()
        {
            // Arrange
            var sut = new CommonDynamicsGateway(Main.Client, Main.DynamicsOptions, new NullLogger<CommonDynamicsGateway>(), null, Main.AuditContext, Main.AuditService);
            var model = "HSBC";// This has been the main testing CMP Creditor and should be present in all environments

            // Act & Asssert
            var result = await sut.SearchCmpCreditors(model);
            CmpCreditorId = result.First().Id;
        }

        public void CheckIfCmpCreditorWasFound()
        {
            if (CmpCreditorId == default)
            {
                Assert.Fail("CmpCreditorId is not present, InitialiseCmpCreditor failed");
            }
        }

        [TestMethod]
        public async Task CmpCreditor_Should_Have_DebtTypes()
        {
            // Arrange
            var model = CmpCreditorId;

            // Act
            var result = await Sut_SharedSvc.GetGenericCreditorById(model);

            // Assert
            Assert.IsNotNull(result.DebtTypes);
        }
    }
}
