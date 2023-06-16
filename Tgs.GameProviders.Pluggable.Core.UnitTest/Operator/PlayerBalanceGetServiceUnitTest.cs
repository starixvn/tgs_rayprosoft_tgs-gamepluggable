using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
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

        decimal gameProviderBalance = 1000m;
        _ = fakePlayerGameBalance.GetBalanceAsync(
            Arg.Is(query.GameProviderCode),
            Arg.Is(query.PlayerCode),
            Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

        var fakeService = new PlayerBalanceGetService(
            fakePlayer,
            fakePlayerGameBalance);

        var actual = await fakeService.GetAsync(query);

        Assert.Equal(gameProviderBalance, actual);
    }
}
