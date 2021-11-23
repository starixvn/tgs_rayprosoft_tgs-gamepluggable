using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Players
{
    [TestClass]
    public class kickplayerGameProviderCodeUnitTest
    {
        [TestMethod]
        public async Task KickPlayerAsync()
        {
            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "ky";

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                    fakeGameProvidersClient
                    .KickPlayerAsync(gameProviderCode, playerCode);

                    fakeGameProvidersClient
                    .KickPlayerAsync(Arg.Is(gameProviderCode), Arg.Is(playerCode));

                    services.AddSingleton(fakeGameProvidersClient);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersPluggableClient(httpClient);

            await sut.KickPlayerAsync(playerCode, gameProviderCode);
        }
    }
}
