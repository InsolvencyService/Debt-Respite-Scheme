using System;
using System.Threading.Tasks;

namespace Insolvency.Interfaces
{
    public interface ICacheClient
    {
        Task<T> GetCachedDataAsync<T>(string key) where T : new();
        Task StoreObjectAsync(string key, object data);
        Task StoreObjectAsync(string key, object data, TimeSpan absoluteExpirationTimeSpan);
    }
}
