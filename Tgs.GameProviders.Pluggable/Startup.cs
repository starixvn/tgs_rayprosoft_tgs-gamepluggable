using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.GameProviders.Pluggable.Core.Extensions;

namespace Tgs.GameProviders.Pluggable
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
            }
            // 啟用 HTTP 記錄
            _ = app.UseHttpLogging();

            //使用IP解析需要使用
            _ = app.UseForwardedHeaders();

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS.GameProviders.PluggableService v1"));

            _ = app.UseHttpsRedirection();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
              {
                  _ = endpoints.MapControllers();
                  _ = endpoints.MapHealthChecks("/healthz");
                  _ = endpoints.MapMetrics();
                  endpoints.MapProfileViewer();
              });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddHttpLogging(httpLogging =>
            {
                httpLogging.LoggingFields = HttpLoggingFields.All;
                httpLogging.RequestBodyLogLimit = 4096;
                httpLogging.ResponseBodyLogLimit = 4096;
            });

            _ = services
                 .AddRouting(options => options.LowercaseUrls = true)
                  .AddControllers()
                  .AddJsonOptions(options =>
                  {
                      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                  });

            _ = services.AddSwaggerGen(c =>
              {
                  var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
                  foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);

                  c.SwaggerDoc("v1",
                      new OpenApiInfo
                      {
                          Title = "Tgs.GameProviders.Pluggable",
                          Version = "v1"
                      });
              });

            _ = services.AddDbTgsRepository((
                 sp,
                 dbBuilder) =>
              {
                  var config = sp.GetRequiredService<IConfiguration>();

                  _ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"));
              });

            _ = services
                 .AddDbBetRepository((
                     sp,
                     dbBuilder) =>
                 {
                     var config = sp.GetRequiredService<IConfiguration>();

                     _ = dbBuilder.UseNpgsql(
                         config.GetConnectionString("Tgs"));
                 });

            _ = services
                 .AddMessageQueue()
                 .AddNatsMessageQueue(natsConfig =>
                 {
                     _ = natsConfig
                     .ConfigQueueOptions((options, sp) =>
                     {
                         var configuration = sp.GetRequiredService<IConfiguration>();
                         options.Url = configuration.GetConnectionString("NatsMQ");
                     });
                 })
                 .ClearAllExchanges()
                 .AddNatsGlobPatternExchange("*");

            _ = services.AddOptions<ForwardedHeadersOptions>()
                 .Configure<IConfiguration>((options, config) =>
                 {
                     options.ForwardedHeaders = ForwardedHeaders.All;
                     options.ForwardLimit = config.GetValue<int>("ForwardedHeaders:ForwardLimit");

                     var knownProxies = config.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>();
                     options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                     // Replace with IP of your proxy/load balancer
                     foreach (var knownProxie in knownProxies)
                         options.KnownProxies.Add(IPAddress.Parse(knownProxie));

                     var knowNetworks = config.GetSection("ForwardedHeaders:KnowNetworks").Get<string[]>() ?? Array.Empty<string>();
                     foreach (var cidr in knowNetworks)
                     {
                         (string ip, string range) = cidr.Split("/");

                         options.KnownNetworks.Add(
                             new IPNetwork(
                                 IPAddress.Parse(ip),
                                 string.IsNullOrEmpty(range)
                                     ? 32
                                     : int.Parse(range)));
                     }
                 });

            _ = services.AddGameProvidersCore(
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:CashServiceBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameProvidersBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameProvidersQueryServerBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameProviderKickPlayerServerBaseUrl");
                  });

            _ = services
                 .AddOptions<OtlpExporterOptions>()
                 .Configure<IConfiguration>((options, conifg) =>
                 {
                     var nodeIp = conifg.GetValue<string>("NODE_IP");

                     if (!string.IsNullOrWhiteSpace(nodeIp))
                         options.Endpoint = new Uri($"http://{nodeIp}:4317");

                     conifg.GetSection("Opentelemetry:Tracing").Bind(options);
                 });

            _ = services.AddOpenTelemetryTracing(builder =>
              {
                  _ = builder
                      .SetResourceBuilder(ResourceBuilder.CreateDefault()
                      .AddEnvironmentVariableDetector())
                      .AddSource("Tgs.*")
                      .AddSource("TGS.*")
                      .AddAspNetCoreInstrumentation()
                      .AddHttpClientInstrumentation()
                      .AddNpgsql()
                      .AddAspNetCoreInstrumentation()
                      .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                      .AddProfileViewExporter()
                      .AddOtlpExporter();
              });

            _ = services.Configure<AspNetCoreInstrumentationOptions>(options =>
              {
                  options.Filter = httpContext =>
                  {
                      return !httpContext.Request.Path.HasValue
                          || !httpContext.Request.Path.StartsWithSegments("/swagger")
                          && !httpContext.Request.Path.StartsWithSegments("/healthz")
                          && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                          && !httpContext.Request.Path.StartsWithSegments("/metrics")
                          && !new[] { "html", "ico", "png", "jpg", "js" }.Any(
                              ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
                  };
              });

            _ = services.AddProfileViewer();

            _ = services
                  .AddInterceptionService()
                  .AddServiceUtiltiyInterceptors();

            _ = services
                  .AddHealthChecks()
                  .ForwardToPrometheus();
        }
    }
}
