using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
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

            _ = app.UseSwagger();
            _ = app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tgs.GameProviders.PluggableService v1"));

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
                 .AddControllers(configure =>
                 {
                     _ = configure.Filters.Add<ExceptionFilter>();
                 })
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
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameVerifysBaseUrl");
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
