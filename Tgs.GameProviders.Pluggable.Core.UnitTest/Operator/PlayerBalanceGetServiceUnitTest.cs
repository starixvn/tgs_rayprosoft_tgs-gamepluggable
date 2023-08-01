using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

public class PlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Player_Balance()
    {
        var fakePlayer = Substitute.For<IPlayer>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeVerification = Substitute.For<IVerification>();
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
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(query.GameProviderCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderId));

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.Kaiyuan,
            UniqueId: 1,
            Name: GameProviderType.Kaiyuan,
            TransferWallet: true,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.Kaiyuan,
            Localization: null);
        _ = fakeGameProvider
            .GetAsync(
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProvider));

        var platformBalance = new PlatformBalanceInfo(
            Balance: 10000m,
            FreeBalance: 1000m,
            OutstandingBalance: 9000m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(query.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(platformBalance);

        var fakeService = new PlayerBalanceGetService(
            fakePlayer,
            fakeGameProvider,
            fakeVerification,
            fakePlayerGameBalance);

        var actual = await fakeService.GetAsync(query);

        Assert.Equal(platformBalance.Balance, actual.Balance);
        Assert.Equal(platformBalance.FreeBalance, actual.FreeBalance);
        Assert.Equal(platformBalance.OutstandingBalance, actual.OutstandingBalance);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerBalanceAsync(
            player: Arg.Is(player),
            gameProvider: Arg.Is(gameProvider),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
