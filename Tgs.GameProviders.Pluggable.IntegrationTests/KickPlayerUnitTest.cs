using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.KickPlayerServer.Client;
using Tgs.GameProviders.Pluggable.Client;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.IntegrationTests
{
    [TestClass]
    public class KickPlayerUnitTest
    {
        [TestMethod]
        public async Task kickplayer_PlayerCodeAsync()
        {
            var playerCode = "4ucmqplxb029";

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeKickPlayerServiceClient = Substitute.For<IKickPlayerServiceClient>();

                    fakeKickPlayerServiceClient
                    .KickPlayerAsync(playerCode);

                    fakeKickPlayerServiceClient
                    .KickPlayerAsync(Arg.Is(playerCode));

                    services.AddSingleton(fakeKickPlayerServiceClient);
                });
            });

            var httpClient = web.CreateClient();
            var faceLogger = NullLogger<GameProvidersPluggableClient>.Instance;

            var sut = new GameProvidersPluggableClient(httpClient, faceLogger);

            await sut.KickPlayerAsync(playerCode);
        }

        [TestMethod]
        public async Task kickplayer_PlayerCode_GameProviderCodeAsync()
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
            var faceLogger = NullLogger<GameProvidersPluggableClient>.Instance;

            var sut = new GameProvidersPluggableClient(httpClient, faceLogger);

            await sut.KickPlayerAsync(playerCode, gameProviderCode);
        }
    }
}