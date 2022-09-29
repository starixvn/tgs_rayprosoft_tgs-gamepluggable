using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;

namespace Tgs.GameProviders.Pluggable.IntegrationTest;

internal class TestPluggableApplication : WebApplicationFactory<Program>
{
    private readonly Action<IWebHostBuilder>? _webHostConfigure;

    public TestPluggableApplication(Action<IWebHostBuilder>? webHostConfigure = null)
    {
        _webHostConfigure = webHostConfigure;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder
            .ConfigureAppConfiguration(c => c.Sources.Clear())
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
                    sp => new ConfigureNamedOptions<OtlpExporterOptions>(
                        Options.DefaultName,
                        exporterOptions =>
                        {
                            exporterOptions.TimeoutMilliseconds = 0;
                        }));
            }); ;

        _webHostConfigure?.Invoke(builder);
    }
}
