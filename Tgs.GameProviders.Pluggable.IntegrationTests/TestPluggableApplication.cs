using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tgs.GameProviders.Pluggable.IntegrationTests
{
    internal class TestCashApplication : WebApplicationFactory<Startup>
    {
        private readonly Action<IWebHostBuilder>? _webHostConfigure;

        public TestCashApplication(Action<IWebHostBuilder>? webHostConfigure = null)
        {
            _webHostConfigure = webHostConfigure;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => _webHostConfigure?.Invoke(builder);
    }
}
