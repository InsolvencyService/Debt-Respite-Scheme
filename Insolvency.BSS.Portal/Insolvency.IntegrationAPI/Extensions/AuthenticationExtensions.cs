using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.IntegrationAPI.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureOpenIdConnect(this IServiceCollection services, Action<JwtBearerOptions> configure)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(opt =>
            {
                configure(opt);
            });

            return services;
        }
    }
}
