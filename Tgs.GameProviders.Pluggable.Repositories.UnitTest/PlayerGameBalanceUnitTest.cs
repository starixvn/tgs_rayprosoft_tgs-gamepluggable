using NSubstitute;
using Tgs.GameCurrency.Client;
using Tgs.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Repositories.UnitTest;

public class PlayerGameBalanceUnitTest
{
    [Fact]
    public async Task GameProvider_Balance_Is_0()
    {
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
        var fakeGameCurrencyClient = Substitute.For<IGameCurrencyClient>();

        var gameProviderCode = "KY";
        var playerCode = "4ucmqplxb029";
        var currencyCode = "CNY";

        var response = new PlayerBalanceResponse(
            Balance: 0,
            FreeBalance: 0,
            OutstandingBalance: 0);
        _ = fakeGameProvidersClient
            .PlayerBalanceAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            playerCode: Arg.Is(playerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

        var repository = new PlayerGameBalance(
            fakeGameProvidersClient,
            fakeGameCurrencyClient);

        var actual = await repository
           .GetBalanceAsync(
            gameProviderCode,
            playerCode,
            currencyCode)
           .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(0m, actual.Balance);
        Assert.Equal(0m, actual.FreeBalance);
        Assert.Equal(0m, actual.OutstandingBalance);

        _ = fakeGameCurrencyClient
            .Received(0)
            .ConvertPlatformBalanceAsync(
            gameProviderCode: Arg.Any<string>(),
            currencyCode: Arg.Any<string>(),
            balance: Arg.Any<decimal>(),
            freeBalance: Arg.Any<decimal>(),
            outstandingBalance: Arg.Any<decimal>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
        var fakeGameCurrencyClient = Substitute.For<IGameCurrencyClient>();

        var gameProviderCode = "KY";
        var playerCode = "4ucmqplxb029";
        var currencyCode = "CNY";

        var response = new PlayerBalanceResponse(
            Balance: 10,
            FreeBalance: 1,
            OutstandingBalance: 9);
        _ = fakeGameProvidersClient
            .PlayerBalanceAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            playerCode: Arg.Is(playerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(response);

        var result = new ConvertPlatformBalanceResult(
            ExchangeRate: 1,
            Balance: 10,
            FreeBalance: 1,
            OutstandingBalance: 9);
        _ = fakeGameCurrencyClient
            .ConvertPlatformBalanceAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            currencyCode: Arg.Is(currencyCode),
            balance: Arg.Is(response.Balance),
            freeBalance: Arg.Is(response.FreeBalance),
            outstandingBalance: Arg.Is(response.OutstandingBalance),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var repository = new PlayerGameBalance(
            fakeGameProvidersClient,
            fakeGameCurrencyClient);

        var actual = await repository
           .GetBalanceAsync(
            gameProviderCode,
            playerCode,
            currencyCode)
           .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(10m, actual.Balance);
        Assert.Equal(1m, actual.FreeBalance);
        Assert.Equal(9m, actual.OutstandingBalance);
    }
}
