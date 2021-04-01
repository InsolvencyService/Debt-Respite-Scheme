using System;
using AutoFixture;
using Insolvency.Integration.Gateways.Audit;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.IntegrationAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Insolvency.RestClient;
using Insolvency.RestClient.ODataBeforeRequestFunctions;
using Simple.OData.Client;

namespace Insolvency.Integration.IntegrationTests
{
    [TestClass]
    public static class Main
    {
        public static IFixture Fixture { get; private set; }
        public static IODataClient Client { get; private set; }
        public static Guid ManagingMoneyOrganisationId { get; set; }
        public static DynamicsGatewayOptions DynamicsOptions { get; private set; }
        public static AuditContext AuditContext { get; private set; }
        public static IAuditService AuditService { get; private set; }

        [AssemblyInitialize]
        public static void Initialise(TestContext context)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            var startUp = new Startup(config);


            var odataClientSettings = startUp.GetODataClientSettings(() => AuditContext,            
                new ODataMessageAuthenticatorFunction(
                new AuthorityDetails
                {
                    ClientId = startUp.Configuration.GetValue<string>("ClientId"),
                    ClientSecret = startUp.Configuration.GetValue<string>("ClientSecret"),
                    ClientUrl = startUp.Configuration.GetValue<string>("AuthorityUrl"),
                    ResourceUrl = startUp.Configuration.GetValue<string>("ClientResource")
                }, null)
            );

            ManagingMoneyOrganisationId = config.GetValue<Guid>("ManagingMoneyOrganisationId");
            Client = new ODataClient(odataClientSettings);
            DynamicsOptions = new DynamicsGatewayOptions();
            config.Bind("DynamicsGateway", DynamicsOptions);
            AuditContext = new AuditContext { ClientId = "tests-integration-dynamics", Name = "Dynamics integration test" };
            Fixture = new Fixture();
            Fixture.Customize(new AllCustomAddresses());
            Fixture.Customize(new AllNominatedContactModel());
            AuditService = new Mock<IAuditService>().Object;
        }
    }
}
