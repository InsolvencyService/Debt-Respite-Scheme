using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Insolvency.Data
{
    public abstract class ContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        public string SettingsFileName { get; set; }

        public ContextFactory(string settingsFileName)
        {
            SettingsFileName = settingsFileName;
        }

        public ContextFactory() : this("app.settings.json") { }

        public virtual TContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(SettingsFileName)
               .Build();

            var connectionString = configuration[ConnectionStringConfigurationKey];
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly(this.GetType().Assembly.FullName));

            return GetContext(optionsBuilder.Options);
        }

        protected abstract TContext GetContext(DbContextOptions<TContext> options);

        protected abstract string ConnectionStringConfigurationKey { get; }
    }
}
