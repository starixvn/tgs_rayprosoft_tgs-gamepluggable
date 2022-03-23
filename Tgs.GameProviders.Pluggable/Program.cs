using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Tgs.GameProviders.Pluggable
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder =>
            {
                builder.AddJsonFile("/config/appsettings.json", true);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((ctx, builder) =>
            {
                builder.AddTgsLog();
                builder.AddOpenTelemetry(options =>
                {
                    options.IncludeFormattedMessage = true;
                    options.ParseStateValues = true;
                    _ = options.AddFluentdForwardExporter(fluentOptions =>
                     {
                         var nodeIp = ctx.Configuration.GetValue<string>("NODE_IP");
                         fluentOptions.Host = nodeIp;
                         fluentOptions.Tag = ctx.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
                         fluentOptions.UseMessagePack();
                     });
                });
            });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
