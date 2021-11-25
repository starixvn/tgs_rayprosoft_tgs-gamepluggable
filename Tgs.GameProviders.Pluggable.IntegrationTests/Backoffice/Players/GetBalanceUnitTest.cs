using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Players
{
    [TestClass]
    public class GetBalanceUnitTest
    {
        [TestMethod]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "ky";
            decimal? gameProviderBalance = 0.88m;
            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                    fakeGameProvidersClient
                    .PlayerBalanceAsync(gameProviderCode, playerCode)
                    .Returns(ValueTask.FromResult(gameProviderBalance));

                    fakeGameProvidersClient
                    .PlayerBalanceAsync(Arg.Is(gameProviderCode), Arg.Is(playerCode))
                    .Returns(ValueTask.FromResult(gameProviderBalance));

                    services.AddSingleton(fakeGameProvidersClient);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            var actual = await sut.GetBalanceAsync(playerCode, gameProviderCode).ConfigureAwait(false);
            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
