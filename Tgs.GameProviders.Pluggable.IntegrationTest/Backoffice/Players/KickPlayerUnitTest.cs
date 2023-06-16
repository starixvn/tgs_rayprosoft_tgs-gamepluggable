using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Players;

public class KickPlayerUnitTest
{
    [Fact]
    public async Task KickPlayerAsync()
    {
        var playerCode = "4ucmqplxb029";

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakeService = Substitute.For<ICommandService<KickPlayerCommand>>();

                var command = new KickPlayerCommand(
                    playerCode);

                _ = fakeService
                .ExecuteAsync(
                    Arg.Is(command),
                    Arg.Any<CancellationToken>());

                _ = services.AddSingleton(fakeService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersBackofficePluggableClient(httpClient);

        await sut.KickPlayerAsync(playerCode);
    }
}
