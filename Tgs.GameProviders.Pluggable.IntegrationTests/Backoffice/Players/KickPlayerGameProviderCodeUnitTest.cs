using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Client;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Players
{
    [TestClass]
    public class KickPlayerGameProviderCodeUnitTest
    {
        [TestMethod]
        public async Task KickPlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "ky";

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                  {
                      var fakseGameProvidersClient = Substitute.For<IGameProvidersClient>();

                      _ = fakseGameProvidersClient.KickPlayerAsync(gameProviderCode, playerCode);

                      _ = fakseGameProvidersClient.KickPlayerAsync(Arg.Is(gameProviderCode), Arg.Is(playerCode));

                      _ = services.AddSingleton(fakseGameProvidersClient);
                  });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            await sut.KickPlayerAsync(playerCode, gameProviderCode);
        }
    }
}
