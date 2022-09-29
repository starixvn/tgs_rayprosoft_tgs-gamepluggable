using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Operator.Players
{
    [TestClass]
    public class GameProviderFundInUnitTest
    {
        [TestMethod]
        public async Task GameProviderFundInAsync()
        {
            string gameProviderCode = GameProviderType.KY;
            string playerCode = "4ucmqplxb029";
            var amount = 1m;

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                  {
                      var fakeFundInCommandService = Substitute.For<ICommandService<FundInCommand>>();

                      var command = new FundInCommand(
                          playerCode,
                          gameProviderCode,
                          amount);
                      _ = fakeFundInCommandService
                      .ExecuteAsync(Arg.Is(command));

                      _ = services.AddSingleton(fakeFundInCommandService);
                  });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            await sut
                .GameProviderFundInAsync(
                    playerCode,
                    gameProviderCode,
                    amount)
                .ConfigureAwait(false);
        }
    }
}
