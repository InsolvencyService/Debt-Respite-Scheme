using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insolvency.Integration.Gateways.Entities;
using Microsoft.Extensions.Logging;
using Simple.OData.Client;

namespace Insolvency.Integration.Gateways.OData
{
    public static class SimpleODataExtensions
    {
        public static async Task<U> ExecuteAsScalarWithLogsAsync<U, T>(this IBoundClient<T> boundClient, ILogger logger) where T : class, IDynamicsEntity
        {
            logger.LogInformation($"Executing Dynamics Query: {await boundClient.GetCommandTextAsync()}");
            var result = await boundClient.ExecuteAsScalarAsync<U>();
            return result;
        }

        public static async Task<T> FindEntryWithLogsAsync<T>(this IBoundClient<T> boundClient, ILogger logger) where T : class, IDynamicsEntity
        {
            logger.LogInformation($"Executing Dynamics Query: {await boundClient.GetCommandTextAsync()}");
            var result = await boundClient.FindEntryAsync();
            return result;
        }

        public static async Task<IEnumerable<T>> FindEntriesWithLogsAsync<T>(this IBoundClient<T> boundClient, ILogger logger, bool resultRequired = true) where T : class, IDynamicsEntity
        {
            logger.LogInformation($"Executing Dynamics Query: {await boundClient.GetCommandTextAsync()}");
            var result = await boundClient.FindEntriesAsync(resultRequired);
            return result;
        }

        public static async Task<T1> InsertEntryWithLogsAsync<T1>(this IBoundClient<T1> boundClient, ILogger logger, bool resultRequired = true) where T1 : class, IDynamicsEntity
        {
            logger.LogDebug($"Executing insert entry {Environment.NewLine} {Environment.StackTrace}");
            logger.LogInformation($"Inserting type: {typeof(T1)}");
            logger.LogInformation($"Executing Dynamics Query: {await boundClient.GetCommandTextAsync()}");
            var result = await boundClient.InsertEntryAsync(resultRequired);
            logger.LogInformation($"Inserted entry with Id: {result.GetId()}");
            return result;
        }

        public static async Task<T1> UpdateEntryWithLogsAsync<T1>(this IBoundClient<T1> boundClient, ILogger logger, bool resultRequired = true) where T1 : class, IDynamicsEntity
        {
            logger.LogDebug($"Executing update entry {Environment.NewLine} {Environment.StackTrace}");
            logger.LogInformation($"Updating type: {typeof(T1)}");
            logger.LogInformation($"Executing Dynamics Query: {await boundClient.GetCommandTextAsync()}");
            var result = await boundClient.UpdateEntryAsync(resultRequired);
            logger.LogInformation($"Updated entry with Id: {result.GetId()}");
            return result;
        }

        public static async Task LinkEntryWithLogsAsync<T1, T2>(this IBoundClient<T1> boundClient, ILogger logger, T2 linkedEntryKey, string linkName)
            where T1 : class, IDynamicsEntity
            where T2 : class, IDynamicsEntity
        {
            logger.LogDebug($"Linking entry {Environment.NewLine} {Environment.StackTrace}");
            logger.LogInformation($"Linking type: {typeof(T1)} to type: {typeof(T2)} with Id: {linkedEntryKey.GetId()}");
            logger.LogInformation($"Executing Dynamics Query: {await boundClient.GetCommandTextAsync()}");
            await boundClient.LinkEntryAsync<T2>(linkedEntryKey, linkName);
            logger.LogInformation($"Link completed successfully.");
        }

        public static IBoundClient<Ntt_breathingspacedebt> ExpandDebt(this IBoundClient<Ntt_breathingspacedebt> debtQuery)
        {
            return debtQuery
                .Expand(x => x.ntt_BreathingSpaceMoratoriumId)
                .Expand(x => x.ntt_debttypeid)
                .Expand(x => x.ntt_creditorid)
                .Expand(x => x.ntt_SoldToCreditorId)
                .Expand(x => x.ntt_breathingspacedebt_ntt_debteligibilityreview_DebtId);
        }
    }
}
