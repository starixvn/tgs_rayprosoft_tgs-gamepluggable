using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tgs.GameProviders.Pluggable.IntegrationTests
{
    internal class TestPluggableApplication : WebApplicationFactory<Startup>
    {
        private readonly Action<IWebHostBuilder>? _webHostConfigure;

        public TestPluggableApplication(Action<IWebHostBuilder>? webHostConfigure = null)
        {
            _webHostConfigure = webHostConfigure;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => _webHostConfigure?.Invoke(builder);
    }
}
