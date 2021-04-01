using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using RestSharp;
using RestSharp.Authenticators;
using Microsoft.Extensions.Logging;
using IdentityModel;
using Insolvency.Common.Identity;
using Insolvency.Interfaces;

namespace Insolvency.RestClient
{
    public abstract class ApiClient : IApiClient
    {
        public TokenResponse Token { get; }
        public IRestClient Client { get; }
        public IRestClientFactory RestFactory { get; }
        public ILogger<ApiClient> Logger { get; }

        public ApiClient(
            IRestClientFactory clientFactory,
            string baseUrl,
            AuthorityDetails authDetails,
            ILogger<ApiClient> logger)
        {
            Client = clientFactory.CreateClient(baseUrl);
            RestFactory = clientFactory;
            if (authDetails != null)
            {
                Token = GetToken(authDetails);
            }
            Logger = logger;
        }

        protected virtual TokenResponse GetToken(AuthorityDetails authDetails)
        {
            var client = RestFactory.CreateClient(authDetails.ClientUrl);
            var request = RestFactory.CreateRequest("token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("resource", authDetails.ResourceUrl);
            if (!string.IsNullOrEmpty(authDetails.Scope))
            {
                request.AddParameter(JwtClaimTypes.Scope, authDetails.Scope);
            }
            client.Authenticator = new HttpBasicAuthenticator(authDetails.ClientId, authDetails.ClientSecret);
            var response = client.Execute<TokenResponse>(request);
            return response?.Data;
        }

        protected IRestRequest BuildRequest<TResult, TModel>(TModel model, string url, Method method)
        {
            var request = RestFactory.CreateRequest(url, method);
            if (model != null)
            {
                request = request.AddJsonBody(model);
            }
            SetupAuthorization(request);
            return request;
        }

        protected IRestRequest BuildRequest<TResult>(string url, Method method)
        {
            var request = RestFactory.CreateRequest(url, method);            
            SetupAuthorization(request);
            return request;
        }

        public async Task<TResult> CreateAsync<TResult, TModel>(TModel model, string url)
        {
            var request = BuildRequest<IRestRequest, TModel>(model, url, Method.POST);
            var response = await Client.ExecuteWithLogsAsync<TResult>(request, Logger);
            return response.Data;
        }

        public async Task<bool> CreateAsync<TModel>(TModel model, string url)
        {
            var request = BuildRequest<IRestRequest, TModel>(model, url, Method.POST);
            var response = await Client.ExecuteWithLogsAsync<object>(request, Logger);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> DeleteAsync(string url)
        {
            var request = BuildRequest<IRestRequest>(url, Method.DELETE);
            var response = await Client.ExecuteWithLogsAsync<object>(request, Logger);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<TResult> UpdateAsync<TResult, TModel>(TModel model, string url)
        {
            var request = BuildRequest<IRestRequest, TModel>(model, url, Method.PUT);
            var response = await Client.ExecuteWithLogsAsync<TResult>(request, Logger);
            return response.Data;
        }

        public async Task<bool> UpdateAsync<TModel>(TModel model, string url)
        {
            var request = BuildRequest<IRestRequest, TModel>(model, url, Method.PUT);
            var response = await Client.ExecuteWithLogsAsync<object>(request, Logger);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public Task<T> CreateAsync<T>(string url) => CreateAsync<T, object>(null, url);

        public async Task<T> GetDataAsync<T>(string url, params KeyValuePair<string, object>[] values)
        {
            var request = BuildRequestDict(url, values);
            var response = await Client.ExecuteWithLogsAsync<T>(request, Logger);
            return response.Data;
        }

        public async Task<bool> GetDataAsync(string url, params KeyValuePair<string, object>[] values)
        {
            var request = BuildRequestDict(url, values);
            var response = await Client.ExecuteWithLogsAsync<object>(request, Logger);
            return response.StatusCode == HttpStatusCode.OK;
        }

        protected IRestRequest BuildRequestDict(string url, params KeyValuePair<string, object>[] values)
        {
            var queryParams = HttpUtility.ParseQueryString("");
            values
                .Where(x => x.Value != null)
                .ToList()
                .ForEach(x => queryParams.Add(x.Key, x.Value.ToString()));
            var uri = $"{url}?{queryParams}";

            var request = RestFactory.CreateRequest(uri, Method.GET);
            SetupAuthorization(request);
            return request;
        }
        protected virtual void SetupAuthorization(IRestRequest request)
        {
            AddAuthorizationHeader(request);
            AddCurrentOrganisationHeader(request);
        }
        protected virtual void AddAuthorizationHeader(IRestRequest request)
        {
            if (Token != null)
            {
                request.AddHeader("Authorization", $"{Token.token_type} {Token.access_token}");
            }
        }
        protected virtual void AddCurrentOrganisationHeader(IRestRequest request)
        {
            var organisationId = GetCurrentOrganisationId();
            if (organisationId.HasValue)
            {
                request.AddHeader(InssHttpHeaderNames.CurrentOrganisationExternalIdHeaderName, organisationId.Value.ToString());
            }
        }

        protected abstract Guid? GetCurrentOrganisationId();
    }
}