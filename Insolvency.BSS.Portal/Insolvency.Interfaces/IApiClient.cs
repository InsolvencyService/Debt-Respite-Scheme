using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insolvency.Interfaces
{
    public interface IApiClient
    {
        Task<TResult> CreateAsync<TResult, TModel>(TModel model, string url);
        Task<TResult> UpdateAsync<TResult, TModel>(TModel model, string url);
        Task<bool> UpdateAsync<TModel>(TModel model, string url);
        Task<bool> CreateAsync<TModel>(TModel model, string url);
        Task<bool> DeleteAsync(string url);
        Task<T> CreateAsync<T>(string url);
        Task<T> GetDataAsync<T>(string url, params KeyValuePair<string, object>[] values);
        Task<bool> GetDataAsync(string url, params KeyValuePair<string, object>[] values);
    }
}
