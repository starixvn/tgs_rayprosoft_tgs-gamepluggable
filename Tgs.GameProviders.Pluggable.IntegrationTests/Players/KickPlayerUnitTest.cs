using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.KickPlayerServer.Client;
using Tgs.GameProviders.Pluggable.Client;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Players
{
    [TestClass]
    public class KickPlayerUnitTest
    {
        [TestMethod]
        public async Task KickPlayerAsync()
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

            var sut = new GameProvidersPluggableClient(httpClient);

            await sut.KickPlayerAsync(playerCode);
        }
    }
}
