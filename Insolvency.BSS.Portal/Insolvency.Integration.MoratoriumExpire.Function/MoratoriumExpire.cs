using System;
using System.Threading.Tasks;
using Insolvency.Integration.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Insolvency.Integration.MoratoriumExpire.Function
{
    public class MoratoriumExpire
    {
        private readonly ICommonDynamicsGateway _dynamicsGateway;

        public MoratoriumExpire(ICommonDynamicsGateway dynamicsGateway)
        {
            _dynamicsGateway = dynamicsGateway;
        }

        [FunctionName("MoratoriumExpire")]
        public async Task Run([TimerTrigger("%CronPattern%")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"MoratoriumExpire function executed at: {DateTime.Now}");

            bool expirePending;

            do
            {
                try
                {
                    expirePending = await _dynamicsGateway.ExpireMoratoriumAsync();
                }
                catch (Exception ex)
                {
                    log.LogError($"An error occurred while expiring moratorium: {ex.Message}");
                    throw;
                }
            }
            while (expirePending);
        }
    }
}
