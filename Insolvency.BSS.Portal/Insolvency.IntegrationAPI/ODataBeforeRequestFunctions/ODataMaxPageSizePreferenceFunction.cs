using System;
using System.Net.Http;
using System.Threading.Tasks;
using Insolvency.Common;
using Insolvency.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Insolvency.IntegrationAPI.ODataBeforeRequestFunctions
{
    public class ODataMaxPageSizePreferenceFunction : IODataBeforeRequestFunction
    {
        private IHttpContextAccessor _httpContextAccessor;

        public ODataMaxPageSizePreferenceFunction(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Task BeforeRequestAsync(HttpRequestMessage message)
        {
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(Constants.MaxPageSizeODataKey))
            {
                message.Headers.Add("Prefer", $"odata.maxpagesize={Constants.PageSize}");
            }
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(Constants.RequestedPageKey))
            {
                int page = (int)_httpContextAccessor.HttpContext.Items[Constants.RequestedPageKey];
                _httpContextAccessor.HttpContext.Items.Remove(Constants.RequestedPageKey);
                message.RequestUri = new Uri($"{message.RequestUri}&$skiptoken=<cookie pagenumber='{page}' />");
            }

            return Task.CompletedTask;
        }
    }
}
