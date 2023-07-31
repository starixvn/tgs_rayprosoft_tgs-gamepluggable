using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

public class PlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Player_Balance()
    {
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var query = new PlayerBalanceQuery(
            "4ucmqplxb029",
            "KY");

        var player = new PlayerInfo(
            Guid.NewGuid(),
            "TGS",
            Guid.NewGuid(),
            "TGS",
            Guid.NewGuid(),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.NewGuid(),
            "CNY");
        _ = fakePlayer
            .GetAsync(
                Arg.Is(query.PlayerCode))
            .Returns(ValueTask.FromResult(player));

        var platformBalance = new PlatformBalanceInfo(
            Balance: 10000m,
            FreeBalance: 1000m,
            OutstandingBalance: 9000m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(query.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(platformBalance);

        var fakeService = new PlayerBalanceGetService(
            fakePlayer,
            fakePlayerGameBalance);

        var actual = await fakeService.GetAsync(query);

        Assert.Equal(platformBalance.FreeBalance, actual);
    }
}
