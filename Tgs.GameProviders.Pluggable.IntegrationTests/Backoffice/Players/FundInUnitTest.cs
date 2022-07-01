using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Players
{
    [TestClass]
    public class FundInUnitTest
    {
        [TestMethod]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "KY";

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    var fakeService = Substitute.For<ICommandService<FundInCommand>>();

                    _ = fakeService.ExecuteAsync(
                        new FundInCommand(
                            PlayerCode: playerCode,
                            GameProviderCode: gameProviderCode));

                    _ = services.AddSingleton(fakeService);
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            await sut.FundInAsync(playerCode, gameProviderCode).ConfigureAwait(false);
        }
    }
}
