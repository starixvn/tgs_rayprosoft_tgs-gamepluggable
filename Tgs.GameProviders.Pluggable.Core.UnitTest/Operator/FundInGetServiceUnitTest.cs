using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

public class FundInGetServiceUnitTest
{
    [Fact]
    public async Task Fund_In()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var query = new FundInCommand(
            "4ucmqplxb029",
            "ky",
            1);

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
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            Arg.Is(query.GameProviderCode),
            Arg.Any<CancellationToken>())
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
            Arg.Is(gameProviderId),
            Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProvider));

        var platformBalance = new PlatformBalanceInfo(
            Balance: 100.0m,
            FreeBalance: 10.0m,
            OutstandingBalance: 90.0m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(platformBalance);

        var fakeService = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakeVerification,
            fakePlayer,
            fakePlayerGameBalance);

        await fakeService
            .ExecuteAsync(query)
            .ConfigureAwait(false);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerAsync(
            player: Arg.Is(player),
            gameProvider: Arg.Is(gameProvider),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundInAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            walletId: Arg.Is(player.DefaultWalletId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            amount: Arg.Is(query.Amount),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GameProvider_SM01_Fund_In_Amount_Only_Integer()
    {
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var query = new FundInCommand(
            "4ucmqplxb029",
            "sm01",
            123M);

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
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            Arg.Is(query.GameProviderCode),
            Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProviderId));

        var gameProvider = new GameProviderInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.SM01,
            UniqueId: 1,
            Name: GameProviderType.SM01,
            TransferWallet: true,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.SM01,
            Localization: null);
        _ = fakeGameProvider
            .GetAsync(
            Arg.Is(gameProviderId),
            Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProvider));

        var platformBalance = new PlatformBalanceInfo(
            Balance: 1000.0m,
            FreeBalance: 123.456m,
            OutstandingBalance: 876.544m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(platformBalance);

        var fakeService = new FundInCommandService(
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakeVerification,
            fakePlayer,
            fakePlayerGameBalance);

        await fakeService
            .ExecuteAsync(query)
            .ConfigureAwait(false);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerAsync(
            player: Arg.Is(player),
            gameProvider: Arg.Is(gameProvider),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundInAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            walletId: Arg.Is(player.DefaultWalletId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            amount: Arg.Is(123M),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
