using Insolvency.Data.Notifications;
using Insolvency.Interfaces.Notifications;
using Insolvency.Notifications.APIFunction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]


namespace Insolvency.Notifications.APIFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetService<IConfiguration>();
                var connectionString = config["PostgresConnection"];
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly("Insolvency.Notifications.Data"));
            });
        }
    }
}
