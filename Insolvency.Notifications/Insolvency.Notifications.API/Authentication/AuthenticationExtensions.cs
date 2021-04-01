using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Notifications.API.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, Action<JwtBearerOptions> configure)
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
