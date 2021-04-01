using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Insolvency.Portal.Extensions;
using Insolvency.RestClient;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insolvency.Portal.Http
{
    public class OrganisationApiClient : ApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrganisationApiClient(
            IRestClientFactory clientFactory,
            string baseUrl,            
            IHttpContextAccessor httpContextAccessor,
            ILogger<OrganisationApiClient> logger) 
        : base(clientFactory, baseUrl, null, logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected override void AddAuthorizationHeader(IRestRequest request)
        {
            var token = GetProfileAccessToken();
            request.AddHeader("Authorization", $"{token.token_type} {token.access_token}");
        }

        protected virtual TokenResponse GetProfileAccessToken()
        {
            var access_token = _httpContextAccessor.HttpContext.GetTokenAsync("access_token").Result;
            return new TokenResponse
            {
                access_token = access_token,
                token_type = "Bearer"
            };
        }

        protected override Guid? GetCurrentOrganisationId()
        {
            return _httpContextAccessor.HttpContext.Session.GetOrganisation()?.Id;
        }
    }
}
