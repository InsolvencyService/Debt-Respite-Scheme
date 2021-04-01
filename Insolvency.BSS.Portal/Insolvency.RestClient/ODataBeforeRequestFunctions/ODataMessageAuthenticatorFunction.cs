using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Insolvency.Interfaces;

namespace Insolvency.RestClient.ODataBeforeRequestFunctions
{
    public class ODataMessageAuthenticatorFunction : IODataBeforeRequestFunction
    {
        private AuthorityDetails _authorityDetails;
        private ICacheClient _cacheClient;

        public const string CachedTokenKey = "AzureADTokenForDynamics";

        public ODataMessageAuthenticatorFunction(AuthorityDetails authorityDetails, ICacheClient cacheClient)
        {
            _authorityDetails = authorityDetails ?? throw new ArgumentNullException(nameof(authorityDetails));
            _cacheClient = cacheClient;
        }

        protected virtual async Task<TokenResponse> GetToken()
        {
            var restFactory = new RestClientFactory();
            var restClient = restFactory.CreateClient(_authorityDetails.ClientUrl);
            var authRequest = restFactory.CreateRequest("token", Method.POST);
            authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            authRequest.AddHeader("Accept", "application/json");
            authRequest.AddParameter("grant_type", "client_credentials");
            authRequest.AddParameter("resource", _authorityDetails.ResourceUrl);
            restClient.Authenticator = new HttpBasicAuthenticator(_authorityDetails.ClientId, _authorityDetails.ClientSecret);
            var response = await restClient.ExecuteAsync<TokenResponse>(authRequest);
            var token = response?.Data;
            return token;
        }

        protected virtual async Task<TokenResponse> GetCachedToken()
        {
            if (_cacheClient == null)
            {
                return null;
            }
            var cachedToken = await _cacheClient.GetCachedDataAsync<TokenResponse>(CachedTokenKey);
            return cachedToken;
        }

        public async Task BeforeRequestAsync(HttpRequestMessage message)
        {
            TokenResponse token;
            var cachedToken = await GetCachedToken();
            if (cachedToken != null && !string.IsNullOrEmpty(cachedToken.access_token))
            {
                token = cachedToken;
            }
            else
            {
                token = await GetToken();
                if (_cacheClient != null)
                {
                    await _cacheClient.StoreObjectAsync(CachedTokenKey, token, TimeSpan.FromMinutes(45));
                }
            }
            message.Headers.Add("Authorization", $"{token.token_type}  {token.access_token}");
        }
    }
}
