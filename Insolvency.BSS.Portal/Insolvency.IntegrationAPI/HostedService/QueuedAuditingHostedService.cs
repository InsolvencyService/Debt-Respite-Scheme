using System;
using System.Threading;
using System.Threading.Tasks;
using Insolvency.Integration.Interfaces;
using Insolvency.Integration.Interfaces.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Insolvency.RestClient;

namespace Insolvency.IntegrationAPI.HostedService
{
    public class QueuedAuditingHostedService : BackgroundService
    {
        private IRestClientFactory RestFactory { get; set; }
        public IBackgroundAuditTaskQueue BackgroundTaskQueue { get; }
        private string BaseUrl { get; set; }
        private ILogger<QueuedAuditingHostedService> Logger { get; set; }

        public QueuedAuditingHostedService(IRestClientFactory restFactory, string baseUrl, ILogger<QueuedAuditingHostedService> logger, IBackgroundAuditTaskQueue backgroundTaskQueue)
        {
            this.RestFactory = restFactory;
            this.BackgroundTaskQueue = backgroundTaskQueue;
            this.BaseUrl = baseUrl;
            this.Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await BackgroundTaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await AuditAsync(workItem);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        private async Task AuditAsync(AuditDetail data)
        {
            var client = RestFactory.CreateClient(BaseUrl);
            var request = RestFactory.CreateRequest("api/InssAuditMessaging", RestSharp.Method.POST);
            request = request.AddJsonBody(data);

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
                Logger.LogWarning($"Auditing Failed: {response.StatusCode}");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
