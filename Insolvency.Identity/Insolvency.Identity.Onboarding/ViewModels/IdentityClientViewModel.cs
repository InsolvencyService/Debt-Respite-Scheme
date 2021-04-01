using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IdentityServer4.Models;
using Insolvency.Interfaces;

namespace Insolvency.Identity.Onboarding.ViewModels
{
    public class IdentityClientViewModel
    {
        private readonly string[] DefaultScopes = { "openid", "inss" };

        [Required]
        public IEnumerable<string> RedirectUris { get; set; }

        [Required]
        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        [Required]
        public string FrontChannelLogoutUri { get; set; }


        public IEnumerable<string> AllowedScopes { get; set; } = new string[0];

        public int? OfflineAccessDurationInDays { get; set; }

        public bool? AllowOfflineAccess { get; set; }
        public int? AccessTokenLifetimeInSeconds { get; set; }

        public bool ShouldUseMfa { get; set; } = false;


        public Client MapToClientEntity(string clientName)
        {
            var newClient = new Client
            {
                AllowedGrantTypes = GrantTypes.Code,
                ClientName = clientName ?? Guid.NewGuid().ToString("N"),
                AllowedScopes = DefaultScopes.Union(AllowedScopes).ToList(),
                AlwaysIncludeUserClaimsInIdToken = true,
                ClientId = $"CodeClientPKCE-{Guid.NewGuid():N}",
                ClientSecrets = new List<Secret>(),
                EnableLocalLogin = false,
                PostLogoutRedirectUris = PostLogoutRedirectUris.ToList(),
                RedirectUris = RedirectUris.ToList(),
                UpdateAccessTokenClaimsOnRefresh = true,
                FrontChannelLogoutUri = FrontChannelLogoutUri
            };

            if (AllowOfflineAccess == true)
            {
                newClient.AllowOfflineAccess = true;
                newClient.AbsoluteRefreshTokenLifetime = Convert.ToInt32(TimeSpan.FromDays(OfflineAccessDurationInDays.Value).TotalSeconds);
                newClient.RefreshTokenUsage = TokenUsage.ReUse;
                newClient.RefreshTokenExpiration = TokenExpiration.Absolute;
            }

            if (AccessTokenLifetimeInSeconds.HasValue)
            {
                newClient.AccessTokenLifetime = AccessTokenLifetimeInSeconds.Value;
            }

            if (ShouldUseMfa)
            {
                newClient.Properties.Add(Constants.ShouldUseMfaKey, "true");
            }

            return newClient;
        }
    }
}
