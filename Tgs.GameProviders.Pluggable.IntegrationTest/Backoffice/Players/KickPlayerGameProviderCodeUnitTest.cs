using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GamePlayerLogOut.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Players;

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
                  var fakeService = Substitute.For<ICommandService<KickPlayerGameProviderCommand>>();

                  var command = new KickPlayerGameProviderCommand(
                      playerCode,
                      gameProviderCode);

                  _ = fakeService
                  .ExecuteAsync(
                      Arg.Is(command),
                      Arg.Any<CancellationToken>());

                  _ = services.AddSingleton(fakeService);
              });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersBackofficePluggableClient(httpClient);

        await sut.KickPlayerAsync(playerCode, gameProviderCode);
    }
}
