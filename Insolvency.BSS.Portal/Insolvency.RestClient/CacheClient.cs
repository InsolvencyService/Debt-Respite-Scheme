using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Insolvency.Interfaces;

namespace Insolvency.RestClient
{
    public class CacheClient : ICacheClient
    {
        public IDistributedCache Cache { get; }
        public TimeSpan DefaultExpirationTimeSpan { get; }

        public CacheClient(IDistributedCache cache, TimeSpan timeSpan)
        {
            this.Cache = cache;
            this.DefaultExpirationTimeSpan = timeSpan;
        }

        public async Task<T> GetCachedDataAsync<T>(string key) where T : new()
        {
            var serializedObject = await this.Cache.GetStringAsync(key);
            if (serializedObject == null)
            {
                return new T();
            }
            return JsonSerializer.Deserialize<T>(serializedObject);
        }

        public async Task StoreObjectAsync(string key, object data)
        {
            await StoreObjectAsync(key, data, DefaultExpirationTimeSpan);
        }

        public async Task StoreObjectAsync(string key, object data, TimeSpan absoluteExpirationTimeSpan)
        {
            var serializedObject = JsonSerializer.Serialize(data);
            await this.Cache.SetStringAsync(key, serializedObject, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationTimeSpan });
        }
    }
}
