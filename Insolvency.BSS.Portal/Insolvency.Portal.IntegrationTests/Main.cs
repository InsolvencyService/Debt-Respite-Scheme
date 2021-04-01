using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Insolvency.IntegrationAPI;
using Insolvency.Interfaces;
using Insolvency.Portal.IntegrationTests.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Insolvency.RestClient;
using Insolvency.RestClient.Experian;
using QAPortTypeClient = Insolvency.RestClient.Experian.QAPortTypeClient;

namespace Insolvency.Portal.IntegrationTests
{
    [TestClass]
    public static class Main
    {
        public static IApiClient ApiClient { get; private set; }

        public static Guid ManagingMoneyOrganisationId { get; set; }

        public static IFixture Fixture { get; private set; }
        public static HttpClient Client { get; private set; }
        public static AddressLookupClient LookupClient { get; private set; }

        private static IConfiguration Configuration { get; set; }

        [AssemblyInitialize]
        public static void Initialise(TestContext context)
        {
            Fixture = new Fixture();

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("testVariables.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            ManagingMoneyOrganisationId = Configuration.GetValue<Guid>("ManagingMoneyOrganisationId");


            SetLookupClient(Configuration.GetValue<string>("ExperianToken"));

            StartUpIntegrationApi(Configuration.GetValue<bool>("isLocalMachine"));

            Fixture.Customize(new AllNominatedContactViewModel());
        }

        private static void StartUpIntegrationApi(bool developmentMachine)
        {
            if (developmentMachine)
            {
                var integrationAPITask = new Task(() => Program.Main(null));
                integrationAPITask.Start();

                while (!Startup.ServerStarted)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void SetCreditorApiCient()
        {
            SetAuthenticatedApiClient("Creditor");
        }

        public static void SetMoneyAdviserCreditorApiCient()
        {
            SetAuthenticatedApiClient("MoneyAdviser");
        }

        private static void SetAuthenticatedApiClient(string authSection)
        {
            var restClientFactory = new RestClientFactory();
            var authDetails = (AuthorityDetails)null;
            if (!string.IsNullOrEmpty(authSection))
            {
                authDetails = new AuthorityDetails();
                Configuration.GetSection("Authentication").GetSection(authSection).Bind(authDetails);
            }

            var integrationApiUrl = Configuration.GetValue<string>("IntegrationAPIUrl");
            Main.ApiClient = new IntegrationApiClient(restClientFactory, integrationApiUrl, authDetails, new NullLogger<IntegrationApiClient>());
        }

        private static void SetLookupClient(string token)
        {
            var tokenInjection = new TokenInjectionInspector(token);
            var tokenInjectorBehavior = new AuthTokenInjectorBehavior(tokenInjection);
            var qAPortTypeClient = new QAPortTypeClient(tokenInjectorBehavior);
            Main.LookupClient = new AddressLookupClient(qAPortTypeClient);
        }
    }
}
