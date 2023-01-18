using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

[TestFixture]
public class GameUrlGetServiceUnitTest
{
    [Test]
    public async Task Game_Url()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
        var fakePlayer = Substitute.For<IPlayer>();

        var query = new GameUrlQuery(
             "4ucmqplxb029",
             Guid.NewGuid(),
             GameProvidersClientPlatformType.Desktop,
             1000001,
             "zh-CN",
             "127.0.0.1",
             new("https://dev-t1.rpgp.cc/"));

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

        _ = fakeGameVerificationClient
            .VerifyPlayerGameUrlAsync(
                Arg.Is(query.PlayerCode),
                Arg.Is(query.GameId))
            .Returns(true);

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.KY,
            GameExternalId: "lobby",
            Type: GameType.CardGameLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.KY,
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: GameProviderType.KY,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.KY);
        _ = fakeGame
            .GetAsync(
                Arg.Is(query.GameId))
            .Returns(ValueTask.FromResult(game));

        var gameProvider = new GameProviderInfo(
            GameProviderId: game.GameProviderId,
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
                Arg.Is(game.GameProviderId))
            .Returns(ValueTask.FromResult(gameProvider));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlAsync(
                Arg.Is(gameProvider.GameProviderCode),
                Arg.Is((GameProvidersClientGameType)game.Type),
                Arg.Is(query.GameId),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameProvider,
            fakeGameUrl,
            fakeGameVerificationClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }

    [Test]
    public async Task Get_AsiaGaming_Game_Url()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
        var fakePlayer = Substitute.For<IPlayer>();

        var query = new GameUrlQuery(
             "4ucmqplxb029",
             Guid.NewGuid(),
             GameProvidersClientPlatformType.Desktop,
             1000001,
             "zh-CN",
             "127.0.0.1",
             new("https://dev-t1.rpgp.cc/"));

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

        _ = fakeGameVerificationClient
            .VerifyPlayerGameUrlAsync(
                Arg.Is(query.PlayerCode),
                Arg.Is(query.GameId))
            .Returns(true);

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AsiaGaming,
            GameExternalId: "lobby",
            Type: GameType.LotteryLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.KY,
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: GameProviderType.AsiaGaming,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.AsiaGaming);
        _ = fakeGame
            .GetAsync(
                Arg.Is(query.GameId))
            .Returns(ValueTask.FromResult(game));
        var gameProvider = new GameProviderInfo(
            GameProviderId: game.GameProviderId,
            GameProviderCode: GameProviderType.AsiaGaming,
            UniqueId: 1,
            Name: GameProviderType.AsiaGaming,
            TransferWallet: true,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.AsiaGaming,
            Localization: null);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(game.GameProviderId))
            .Returns(ValueTask.FromResult(gameProvider));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlByAsiaGamingAsync(
                Arg.Is(gameProvider.GameProviderCode),
                Arg.Is((GameProvidersClientGameType)game.Type),
                Arg.Is(query.GameId),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl),
                Arg.Is(player.DefaultWalletId))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameProvider,
            fakeGameUrl,
            fakeGameVerificationClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }

    [Test]
    public async Task Get_BetConstruct_Game_Url()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameProvider = Substitute.For<IGameProvider>();
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeGameVerificationClient = Substitute.For<IGameVerificationClient>();
        var fakePlayer = Substitute.For<IPlayer>();

        var query = new GameUrlQuery(
             "4ucmqplxb029",
             Guid.NewGuid(),
             GameProvidersClientPlatformType.Desktop,
             1000001,
             "zh-CN",
             "127.0.0.1",
             new("https://dev-t1.rpgp.cc/"));

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

        _ = fakeGameVerificationClient
            .VerifyPlayerGameUrlAsync(
                Arg.Is(query.PlayerCode),
                Arg.Is(query.GameId))
            .Returns(true);
        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.BetConstruct,
            GameExternalId: "lobby",
            Type: GameType.Sportsbook,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.KY,
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: GameProviderType.BetConstruct,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.BetConstruct);
        _ = fakeGame
            .GetAsync(
                Arg.Is(query.GameId))
            .Returns(ValueTask.FromResult(game));

        var gameProvider = new GameProviderInfo(
            GameProviderId: game.GameProviderId,
            GameProviderCode: GameProviderType.BetConstruct,
            UniqueId: 1,
            Name: GameProviderType.BetConstruct,
            TransferWallet: false,
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            Description: GameProviderType.BetConstruct,
            Localization: null);
        _ = fakeGameProvider
            .GetAsync(
                Arg.Is(game.GameProviderId))
            .Returns(ValueTask.FromResult(gameProvider));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlByBetConstructAsync(
                Arg.Is(gameProvider.GameProviderCode),
                Arg.Is((GameProvidersClientGameType)game.Type),
                Arg.Is(query.GameId),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl),
                Arg.Is(player.BrandId),
                Arg.Is(gameProvider.GameProviderId))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameProvider,
            fakeGameUrl,
            fakeGameVerificationClient,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameUrl, actual);
    }
}
