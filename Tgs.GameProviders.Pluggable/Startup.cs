using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS.GameProviders.PluggableService v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResultStatusCodes = new Dictionary<HealthStatus, int>
                    {
                        { HealthStatus.Healthy, StatusCodes.Status200OK },
                        { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable },
                        { HealthStatus.Degraded, StatusCodes.Status500InternalServerError }
                    },
                    ResponseWriter = async (context, report) =>
                    {
                        var hostName = Dns.GetHostName();
                        var hostEntry = await Dns.GetHostEntryAsync(hostName);
                        var hostIp = Array.Find(hostEntry.AddressList,
                            x => x.AddressFamily == AddressFamily.InterNetwork);

                        var response = new
                        {
                            Data = new
                            {
                                Environment.MachineName,
                                HostName = hostName,
                                HostIp = hostIp?.ToString() ?? string.Empty,
                                Environment = env.EnvironmentName,
                                OsVersion = $"{Environment.OSVersion}",
                                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                ProcessCount = Environment.ProcessorCount
                            },
                            Result = new
                            {
                                Status = report.Status.ToString(),
                                TotalDuration = report.TotalDuration.TotalSeconds,
                                DependencyServices = report.Entries.Select(srv => new
                                {
                                    srv.Key,
                                    Duration = srv.Value.Duration.TotalSeconds,
                                    srv.Value.Description,
                                    srv.Value.Data,
                                    Exception = srv.Value.Exception?.Message,
                                    Status = srv.Value.Status.ToString(),
                                    Tags = srv.Value.Tags.Any()
                                        ? string.Join(',', srv.Value.Tags)
                                        : string.Empty
                                }).ToArray()
                            }
                        };
                        var json = JsonSerializer.Serialize(response,
                            new JsonSerializerOptions { WriteIndented = true });
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json);
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting(options => options.LowercaseUrls = true)
                 .AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                 });
            services.AddSwaggerGen(c =>
            {
                var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
                foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "TGS.GameProviders",
                        Version = "v1"
                    });
            });

            services.AddDbTgsRepository((
               sp,
               dbBuilder) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();

                dbBuilder
                    .UseLazyLoadingProxies();

                dbBuilder.UseNpgsql(
                    config.GetConnectionString("Tgs"));
            });

            services
                .AddDbBetRepository((
                    sp,
                    dbBuilder) =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();

                    dbBuilder
                        .UseLazyLoadingProxies();

                    dbBuilder.UseNpgsql(
                        config.GetConnectionString("Tgs"));
                });

            services.AddGameProvidersClient((options, sp) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                options.BaseUrl = new Uri(configuration.GetConnectionString("TGSGameProviders"));
                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                };
            });

            services.AddCashServiceClient((options, sp) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                options.BaseUrl = new(configuration.GetConnectionString("CashService"));

                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                };
            });

            services.AddGameProvidersServices()
                .AddGameProvidersCore();

            services
                .AddHealthChecks()
                .AddNpgSql(Configuration.GetConnectionString("Tgs"), name: "PostgreSQL Tgs")
                .AddUrlGroup(new Uri($"{Configuration.GetConnectionString("TGSGameProviders")}/health"), "TGS Game Providers")
                .AddUrlGroup(new Uri($"{Configuration.GetConnectionString("CashService")}/health"), "Cash Service");
        }
    }
}
