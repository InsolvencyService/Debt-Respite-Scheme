// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using IdentityModel;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Insolvency.Data.IdentityManagement;
using Insolvency.Identity.Models.Claims;

namespace Insolvency.Data.IdentityServer
{
    public static class IdentityServerSeed
    {
        public static void SeedConfigurationData(IdentityManagementContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResource("inss", InsolvencyClaims)
            };

        public static IEnumerable<ApiScope> ApiScopes
        {
            get
            {
                var apiScopes = new List<ApiScope>();
                apiScopes.Add(new ApiScope("example", "Example API"));                
                apiScopes.AddRange(IntegrationApiScopes);
                apiScopes.AddRange(NotificationApiScopes);
                return apiScopes;
            }
        }


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "ClientIdExample",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "example", "openid" },
                    RequireClientSecret = true
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("bss.api", "BSS Integration API", InsolvencyClaims)
                {
                     Scopes = IntegrationApiScopes.Select(scope => scope.Name).ToList()
                },
                new ApiResource("bss.notification.api", "BSS Integration API", InsolvencyClaims)
                {
                     Scopes = NotificationApiScopes.Select(scope => scope.Name).ToList()
                }
            };

        public static IEnumerable<string> InsolvencyClaims =>
            new List<string>
            {
                InssClaimTypes.ScpGroupId,
                JwtClaimTypes.Name,
                InssClaimTypes.Organisation,
                JwtClaimTypes.Email,
                JwtClaimTypes.EmailVerified,
                JwtClaimTypes.Profile,
                InssClaimTypes.GroupProfile
            };

        public static IEnumerable<ApiScope> IntegrationApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("bss.creditor.api", "Integration API: BSS Creditor Access"),
                new ApiScope("bss.moneyadviser.api", "Integration API: BSS Creditor Access")
            };

        public static IEnumerable<ApiScope> NotificationApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("bss.notification.api", "Notification API")
            };
    }
}
