using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

[TestFixture]
public class PlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Player_Balance()
    {
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var query = new PlayerBalanceQuery(
            "4ucmqplxb029",
            "KY");

        var player = new PlayerInfo(
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
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

        Assert.AreEqual(gameProviderBalance, actual);
    }
}
