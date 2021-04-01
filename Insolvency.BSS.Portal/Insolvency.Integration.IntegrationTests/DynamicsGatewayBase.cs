using System;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.Integration.Gateways;
using Insolvency.Integration.Gateways.Mapper;
using Insolvency.Integration.Models;
using Insolvency.Integration.Models.MoneyAdviserService.Requests;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Insolvency.Integration.IntegrationTests
{
    public class DynamicsGatewayBase
    {
        public static Guid MoratoriumId { get; set; }
        public MoneyAdviserServiceDynamicsGateway Sut_MoneyAdvSvc { get; set; }
        public CreditorServiceDynamicsGateway Sut_CreditorSvc { get; set; }
        public CommonDynamicsGateway Sut_SharedSvc { get; set; }

        public virtual void TestInit()
        {
            CheckIfDebtorWasCreated();
            SetSutMoneyAdvSvc();
            SetSutCreditorSvc();
            SetSutShared();
        }

        public static async Task InitialiseDebtorAsync()
        {
            // Arrange          

            var sut = new MoneyAdviserServiceDynamicsGateway(Main.Client, Main.DynamicsOptions, new NullLogger<MoneyAdviserServiceDynamicsGateway>(), Main.AuditContext, Main.AuditService);
            var model = Main.Fixture.Create<ClientDetailsCreateRequest>();
            model.DateOfBirth = model.DateOfBirth.AddYears(-20);

            // Act & Asssert
            var result = await sut.CreateClientWithDetailsAsync(model, Main.ManagingMoneyOrganisationId);
            MoratoriumId = result;
        }

        public async Task SetDebtorAddress()
        {
            var model = Main.Fixture.Build<CustomAddress>()
              .Do(x => x.OwnerId = MoratoriumId)
              .Without(x => x.Postcode)
              .Without(x => x.OwnerId)
              .Without(x => x.DateFrom)
              .Without(x => x.DateTo)
              .Create();
            model.Postcode = "NW10 2AB";

            await Sut_MoneyAdvSvc.CreateDebtorAddressAsync(model);
        }

        public void CheckIfDebtorWasCreated()
        {
            if (MoratoriumId == default)
            {
                Assert.Fail("MoratoriumId is not present, InitialiseDebtor failed");
            }
        }

        public async Task AddAdHocCreditorAndDebt()
        {
            var creditorName = Main.Fixture.Create<string>();
            var createAdHocModel = Main.Fixture.Build<CreateDebtRequest>()
                  .Without(x => x.CreditorId)
                  .Without(x => x.DebtTypeId)
                  .Create();

            // Act & Assert
            await InitialiseDebtorAsync();
            createAdHocModel.MoratoriumId = MoratoriumId;

            var creditorId = await Sut_SharedSvc.CreateAdHocCreditor(creditorName);
            createAdHocModel.CreditorId = creditorId;

            await Sut_MoneyAdvSvc.CreateDebtAsync(createAdHocModel);
        }

        public void SetSutMoneyAdvSvc() => Sut_MoneyAdvSvc = new MoneyAdviserServiceDynamicsGateway(Main.Client, Main.DynamicsOptions, new NullLogger<MoneyAdviserServiceDynamicsGateway>(), Main.AuditContext, Main.AuditService);
        public void SetSutCreditorSvc() => Sut_CreditorSvc = new CreditorServiceDynamicsGateway(Main.Client, new NullLogger<CreditorServiceDynamicsGateway>(), Main.AuditContext, Main.AuditService);
        public void SetSutShared() => Sut_SharedSvc = new CommonDynamicsGateway(Main.Client, Main.DynamicsOptions, new NullLogger<CommonDynamicsGateway>(), new MapperService(), Main.AuditContext, Main.AuditService);

    }
}
