using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tgs.CashService.Client;
using Tgs.GameCommand.Common;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

public class FundOutGetServiceUnitTest
{
    [Fact]
    public async Task Fund_Out()
    {
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundOutCommand(
            PlayerCode: "4ucmqplxb029",
            GameProviderCode: "ky",
            Amount: 1);

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
            playerCode: Arg.Is(command.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
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
            Balance: 1000m,
            FreeBalance: 0m,
            OutstandingBalance: 1000m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(platformBalance);

        var balance = 10.0m;
        _ = fakeCashServiceClient
            .GetBalanceAsync(
            walletId: Arg.Is(player.DefaultWalletId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(balance));

        var fakeService = new FundOutCommandService(
            fakeCashServiceClient,
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakeVerification,
            fakePlayer,
            fakePlayerGameBalance);

        await fakeService.ExecuteAsync(command);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerFundAsync(
            player: Arg.Is(player),
            gameProvider: Arg.Is(gameProvider),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundOutAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            walletId: Arg.Is(player.DefaultWalletId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            amount: Arg.Is(command.Amount),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GameProvider_SM01_Fund_Out_Amount_Only_Integer()
    {
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundOutCommand(
            GameProviderType.SM01,
            "4ucmqplxb029",
            123.45M);

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
                Arg.Is(command.PlayerCode))
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(command.GameProviderCode))
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
                Arg.Is(gameProviderId))
            .Returns(ValueTask.FromResult(gameProvider));

        var platformBalance = new PlatformBalanceInfo(
            Balance: 1000M,
            FreeBalance: 0M,
            OutstandingBalance: 1000m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(platformBalance);

        var balance = 1000.0M;
        _ = fakeCashServiceClient
            .GetBalanceAsync(
                Arg.Is(player.DefaultWalletId))
            .Returns(ValueTask.FromResult(balance));

        var fakeService = new FundOutCommandService(
            fakeCashServiceClient,
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakeVerification,
            fakePlayer,
            fakePlayerGameBalance);

        await fakeService.ExecuteAsync(command);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerFundAsync(
               player: Arg.Is(player),
               gameProvider: Arg.Is(gameProvider),
               cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundOutAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
                brandCode: Arg.Is(player.BrandCode),
                playerCode: Arg.Is(player.PlayerCode),
                walletId: Arg.Is(player.DefaultWalletId),
                currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
                amount: Arg.Is(123M),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Timeout_Exception()
    {
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();
        var fakePlayerGameBalance = Substitute.For<IPlayerGameBalance>();

        var command = new FundOutCommand(
            PlayerCode: "4ucmqplxb029",
            GameProviderCode: "ky",
            Amount: 1);

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
            playerCode: Arg.Is(command.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
            gameProviderCode: Arg.Is(command.GameProviderCode),
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
            Balance: 1000m,
            FreeBalance: 0m,
            OutstandingBalance: 1000m);
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(platformBalance);

        _ = fakeCashServiceClient
            .GetBalanceAsync(
            walletId: Arg.Is(player.DefaultWalletId),
            cancellationToken: Arg.Any<CancellationToken>())
            .ThrowsAsync(new TimeoutException());

        var fakeService = new FundOutCommandService(
            fakeCashServiceClient,
            fakeGameFundTransferClient,
            fakeGameProvider,
            fakeVerification,
            fakePlayer,
            fakePlayerGameBalance);

        _ = await Assert.ThrowsAsync<TimeoutException>(() => fakeService.ExecuteAsync(command).AsTask());

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerFundAsync(
            player: Arg.Is(player),
            gameProvider: Arg.Is(gameProvider),
            cancellationToken: Arg.Any<CancellationToken>());

        _ = fakeGameFundTransferClient
            .Received(0)
            .FundOutAsync(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProvider.GameProviderCode),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            walletId: Arg.Is(player.DefaultWalletId),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            amount: Arg.Is(command.Amount),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
