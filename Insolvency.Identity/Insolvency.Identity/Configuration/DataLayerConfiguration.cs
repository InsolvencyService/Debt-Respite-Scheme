using System;
using System.Reflection;
using IdentityServer4.EntityFramework.Options;
using Insolvency.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Insolvency.Identity.Configuration
{
    public static class DataLayerConfiguration
    {
        static string identityServerStoreMigrationAsembly = typeof(ContextFactory<DbContext>).GetTypeInfo().Assembly.GetName().Name;


        public static void ConfigureConfigurationStore(ConfigurationStoreOptions options)
        {
            options.ResolveDbContextOptions = ResolveDbContextOptions;
        }

        public static void ConfigureOperationalStore(OperationalStoreOptions options)
        {
            options.ResolveDbContextOptions = ResolveDbContextOptions;
            options.EnableTokenCleanup = true;
            options.TokenCleanupInterval = Constants.AuthenticationLifeTimeFromHours;
        }

        public static void ConfigureIdentityManagementContext(IServiceProvider services, DbContextOptionsBuilder options)
        {
            var connectionString = services.GetService<IConfiguration>().GetConnectionString("IdentityConnection");
            options.UseNpgsql(connectionString);
        }

        private static void ResolveDbContextOptions(IServiceProvider services, DbContextOptionsBuilder builder)
        {
            var connectionString = services.GetService<IConfiguration>().GetConnectionString("IdentityConnection");
            builder.UseNpgsql(connectionString, dbOpts => dbOpts.MigrationsAssembly(identityServerStoreMigrationAsembly));
        }
    }
}
