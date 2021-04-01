using System;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Integration.Interfaces;
using Insolvency.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Insolvency.Integration.CMPListLoad
{
    public class CMPCacheLoader
    {
        public ICacheClient CacheClient { get; }
        public ICommonDynamicsGateway DynamicsGateway { get; }
        public ILogger<CMPCacheLoader> Logger { get; }

        public CMPCacheLoader(ICommonDynamicsGateway dynamicsGateway, ICacheClient cacheClient, ILogger<CMPCacheLoader> logger)
        {
            this.CacheClient = cacheClient;
            this.DynamicsGateway = dynamicsGateway;
            this.Logger = logger;
        }

        [FunctionName("CMPCacheLoader")]
        public async Task Run([TimerTrigger("%CronPattern%", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            Logger.LogInformation($"C# Timer trigger web job executed at: {DateTime.Now}");

            var result = await this.DynamicsGateway.GetAllCMPCreditors();
            await this.CacheClient.StoreObjectAsync(Constants.CMPCacheListKey, result);
        }
    }
}
