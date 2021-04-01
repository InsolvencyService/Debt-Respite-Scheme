using System;
using Insolvency.Audit.MessageProcessor.Function;
using Insolvency.Data.Audit;
using Insolvency.Interfaces.Audit;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Insolvency.Audit.MessageProcessor.Function
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddScoped<IAuditRepository, AuditRepository>();

            builder.Services.AddDbContext<AuditDbContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var connectionString = config["AuditDbConnection"];
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly("Insolvency.Data"));
            });

            var connectionString = Environment.GetEnvironmentVariable("AuditDbConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();
            optionsBuilder.UseNpgsql(connectionString, x => x.MigrationsAssembly("Insolvency.Data"));
            var context = new AuditDbContext(optionsBuilder.Options);
            AuditDbInitialise.Init(context);
        }
    }
}