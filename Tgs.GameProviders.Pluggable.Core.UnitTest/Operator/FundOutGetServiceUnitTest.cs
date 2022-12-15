using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameFundTransfer.Client;
using Tgs.GameFundTransfer.Protos;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
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
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
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
            gameProviderId,
            GameProviderType.KY);
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
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
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
            gameProviderId,
            GameProviderType.SM01);
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
