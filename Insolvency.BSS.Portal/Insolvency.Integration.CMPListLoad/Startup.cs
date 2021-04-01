using System;
using Insolvency.Common;
using Insolvency.Integration.CMPListLoad;
using Insolvency.Integration.Gateways;
using Insolvency.Integration.Gateways.OData;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Models;
using Insolvency.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Insolvency.RestClient;
using Insolvency.RestClient.ODataBeforeRequestFunctions;
using Simple.OData.Client;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Insolvency.Integration.CMPListLoad
{
    //IWebJobsStartup or IWebJobsConfigurationStartup
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var dynamicsOptions = new DynamicsGatewayOptions();
            builder.Services.AddScoped<ICommonDynamicsGateway, CommonDynamicsGateway>();
            builder.Services.AddSingleton<DynamicsGatewayOptions>(x =>
            {
                var config = x.GetService<IConfiguration>();
                config.GetSection("DynamicsGateway").Bind(dynamicsOptions);
                return dynamicsOptions;
            });

            builder.Services.AddSingleton<IDistributedCache, RedisCache>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var redisConnectionString = config["redisServerUrl"];
                var options = new RedisCacheOptions
                {
                    Configuration = redisConnectionString,
                    InstanceName = Constants.IntegrationAPICacheKey
                };
                return new RedisCache(options);
            });

            builder.Services.AddScoped<IODataClient, ODataClient>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var oDataBeforeFunction = new ODataMessageAuthenticatorFunction(
                   new AuthorityDetails
                   {
                       ClientId = config.GetValue<string>("ClientId"),
                       ClientSecret = config.GetValue<string>("ClientSecret"),
                       ClientUrl = config.GetValue<string>("AuthorityUrl"),
                       ResourceUrl = config.GetValue<string>("ClientResource")
                   }, x.GetService<ICacheClient>());
                var clientSettings = GetODataClientSettings(config, oDataBeforeFunction);
                return new ODataClient(clientSettings);
            });
            builder.Services.AddScoped<ICacheClient, CacheClient>(x =>
            {
                var config = x.GetService<IConfiguration>();
                var listLifetimeInDays = config.GetValue<int>("CMPListCacheLifeTimeInDays");
                return new CacheClient(x.GetService<IDistributedCache>(), TimeSpan.FromDays(listLifetimeInDays));
            });           
        }

        public ODataClientSettings GetODataClientSettings(IConfiguration config, IODataBeforeRequestFunction oDataBeforeFunction)
        {
            var dynamicsUrl = config.GetValue<string>("DynamicsUrl");

            var restFactory = new RestClientFactory();
            var clientSettings = new ODataClientSettings
            {
                BaseUri = new Uri(dynamicsUrl),
                BeforeRequestAsync = message => oDataBeforeFunction.BeforeRequestAsync(message),
                PayloadFormat = ODataPayloadFormat.Json
            };
            clientSettings.AdapterFactory = new CustomCreteAdapterLoaderFactory(
                (x, y) => new CompositeMetadataODataAdapter(x, y,
                    z => new WrapperMedataOmitNullValues(z)));
            return clientSettings;
        }
    }
}
