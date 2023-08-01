using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Players;

public class GetBalanceInfoUnitTest
{
    [Fact]
    public async Task GetBalanceAsync()
    {
        var playerCode = "4ucmqplxb029";
        var gameProviderCode = "VR";

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
            {
                var fakePlayerBalanceGetService = Substitute.For<IGetService<PlayerBalanceQuery, PlayerBalanceResult>>();

                var query = new PlayerBalanceQuery(
                    playerCode,
                    gameProviderCode);
                var result = new PlayerBalanceResult(
                    Balance: 100m,
                    FreeBalance: 10m,
                    OutstandingBalance: 90m);

                _ = fakePlayerBalanceGetService
                    .GetAsync(
                    query: Arg.Is(query),
                    cancellationToken: Arg.Any<CancellationToken>())
                    .Returns(result);

                _ = services.AddSingleton(fakePlayerBalanceGetService);
            });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersBackofficePluggableClient(httpClient);

        var actual = await sut.GetBalanceInfoAsync(playerCode, gameProviderCode).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(100m, actual.Balance);
        Assert.Equal(10m, actual.FreeBalance);
        Assert.Equal(90m, actual.OutstandingBalance);
    }
}
