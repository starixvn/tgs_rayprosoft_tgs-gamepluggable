using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
        public void Configure(IApplicationBuilder app)
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
              {
                  _ = endpoints.MapControllers();
                  _ = endpoints.MapHealthChecks("/healthz");
                  endpoints.MapProfileViewer();
              });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
              });

            _ = services.AddDbBetRepository(
                (sp, dbBuilder) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    _ = dbBuilder.UseNpgsql(
                        config.GetConnectionString("Tgs"));
                });

            _ = services.AddGameProvidersCore(
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:CashServiceClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameProvidersClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameProvidersQueryServerClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:KickPlayerServiceClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameVerificationClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameFundTransferClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = configuration.GetValue<Uri>("Pluggable:GameCurrencyClientBaseUrl");
                  },
                  (options, sp) =>
                  {
                      var configuration = sp.GetRequiredService<IConfiguration>();
                      options.BaseUrl = new(configuration.GetSection("Pluggable:GameRoundsClientBaseUrl").Value);
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
                      .AddGrpcClientInstrumentation()
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
                  .AddHealthChecks();
        }
    }
}
