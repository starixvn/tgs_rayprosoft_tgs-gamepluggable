using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameCommand.Common;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

[TestFixture]
public class FundOutGetServiceUnitTest
{
    [Test]
    public async Task Fund_Out()
    {
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
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
                Arg.Is(query.PlayerCode))
            .Returns(ValueTask.FromResult(player));

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProvider
            .GetGameProviderIdAsync(
                Arg.Is(query.GameProviderCode))
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
                Arg.Is(gameProviderId))
            .Returns(ValueTask.FromResult(gameProvider));

        _ = fakeGameVerificationClient
            .VerifyPlayerFundAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProvider.GameProviderId))
            .Returns(true);

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
            fakeGameVerificationClient,
            fakePlayer,
            fakePlayerGameBalance);

        await fakeService.ExecuteAsync(query).ConfigureAwait(false);

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundOutAsync(Arg.Any<GameFundTransferFundRequest>());
    }

    [Test]
    public async Task GameProvider_SM01_Fund_Out_Amount_Only_Integer()
    {
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeGameFundTransferClient = Substitute.For<IGameFundTransferClient>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
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

        _ = fakeGameVerificationClient
            .VerifyPlayerFundAsync(
                Arg.Is(player.PlayerCode),
                Arg.Is(gameProvider.GameProviderId))
            .Returns(true);

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
            fakeGameVerificationClient,
            fakePlayer,
            fakePlayerGameBalance);

        await fakeService.ExecuteAsync(query).ConfigureAwait(false);

        var fundRequest = new GameFundTransferFundRequest
        {
            Amount = 123M,
            BrandCode = "GPI",
            CurrencyCode = "CNY",
            GameProviderCode = GameProviderType.SM01,
            GameProviderId = gameProvider.GameProviderId,
            PlayerCode = player.PlayerCode,
            WalletId = player.DefaultWalletId
        };

        _ = fakeGameFundTransferClient
            .Received(1)
            .FundOutAsync(Arg.Is(fundRequest));

        Assert.AreEqual(123M, fundRequest.Amount.ToDecimal());
    }
}
