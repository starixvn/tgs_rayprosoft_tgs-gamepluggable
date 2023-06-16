using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.GameCommand.Common;
using Tgs.GameFundTransfer.Client;
using Tgs.GameProviders.Domains;
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

        var query = new FundOutCommand(
            GameProviderType.KY,
            "4ucmqplxb029",
            1
            );

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
            GameProviderCode: GameProviderType.KY,
            UniqueId: 1,
            Name: GameProviderType.KY,
            TransferWallet: true,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.KY,
            Localization: null);
        _ = fakeGameProvider
            .GetAsync(
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(gameProvider));

        decimal gameProviderBalance = 0m;
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(ValueTask.FromResult(gameProviderBalance));

        var balance = 10.0m;
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

        await fakeService.ExecuteAsync(query).ConfigureAwait(false);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerAsync(
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
                amount: Arg.Is(query.Amount),
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

        var query = new FundOutCommand(
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
                Arg.Is(query.PlayerCode))
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(query.GameProviderCode))
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

        decimal gameProviderBalance = 0M;
        _ = fakePlayerGameBalance
            .GetBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode))
            .Returns(ValueTask.FromResult(gameProviderBalance));

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

        await fakeService.ExecuteAsync(query).ConfigureAwait(false);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerAsync(
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
}
