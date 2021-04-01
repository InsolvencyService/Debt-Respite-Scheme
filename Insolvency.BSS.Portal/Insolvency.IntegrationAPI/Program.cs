using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Insolvency.IntegrationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            UseStartup(builder).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);
                

        public static IHostBuilder UseStartup(IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}
