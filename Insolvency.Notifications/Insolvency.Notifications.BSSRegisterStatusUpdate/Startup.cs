using Insolvency.Notifications.BSSRegisterStatusUpdate;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;

[assembly: FunctionsStartup(typeof(Startup))]


namespace Insolvency.Notifications.BSSRegisterStatusUpdate
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IRestClientFactory, RestClientFactory>();
            builder.Services.AddScoped<IRestClient, RestClient>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var dynamicsUrl = config["DynamicsUrl"];
                return new RestClient(dynamicsUrl);
            });

            builder.Services.AddScoped<AuthorityDetails>(serviceProvider =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                return new AuthorityDetails
                {
                    ClientId = config["ClientId"],
                    ClientSecret = config["ClientSecret"],
                    ClientUrl = config["AuthorityUrl"],
                    ResourceUrl = config["ClientResource"]
                };
            });

        }
    }
}
