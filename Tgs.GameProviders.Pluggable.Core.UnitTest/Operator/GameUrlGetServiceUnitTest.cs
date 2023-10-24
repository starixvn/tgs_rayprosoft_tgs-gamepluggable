using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator;

public class GameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Game_Url()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();

        var query = new GameUrlQuery(
             PlayerCode: "4ucmqplxb029",
             LoginRecordId: Guid.NewGuid(),
             PlatformType: GameProvidersClientPlatformType.Desktop,
             GameId: 1000001,
             Language: "zh-CN",
             IpAddress: "127.0.0.1",
             BackUrl: new("https://dev-t1.rpgp.cc/"));

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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.Kaiyuan,
            GameExternalId: "lobby",
            Type: GameType.CardGameLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.Kaiyuan,
            SubGame: true,
            AvailableToBrand: true,
            HasTurnoverOnTieBet: true,
            Html5: true,
            Description: GameProviderType.Kaiyuan,
            EndpointPath: null,
            RuleUrl: null,
            AvailableCurrencies: null,
            Localization: null,
            SubGameProviderCode: GameProviderType.Kaiyuan);
        _ = fakeGame
            .GetAsync(
            gameId: Arg.Is(query.GameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(game));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            gameType: Arg.Is((GameProvidersClientGameType)game.Type),
            gameId: Arg.Is(game.GameId),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            loginRecordId: Arg.Is(query.LoginRecordId),
            platformType: Arg.Is(query.PlatformType),
            language: Arg.Is(query.Language),
            ipAddress: Arg.Is(query.IpAddress),
            backUrl: Arg.Is(query.BackUrl))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameUrl,
            fakeVerification,
            fakePlayer);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerGameUrlAsync(
            player: Arg.Is(player),
            game: Arg.Is(game),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Get_AsiaGaming_Game_Url()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeVerification = Substitute.For<IVerification>();
        var fakePlayer = Substitute.For<IPlayer>();

        var query = new GameUrlQuery(
             PlayerCode: "4ucmqplxb029",
             LoginRecordId: Guid.NewGuid(),
             PlatformType: GameProvidersClientPlatformType.Desktop,
             GameId: 1000001,
             Language: "zh-CN",
             IpAddress: "127.0.0.1",
             BackUrl: new("https://dev-t1.rpgp.cc/"));

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

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.AsiaGaming,
            GameExternalId: "lobby",
            Type: GameType.LotteryLobby,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.Kaiyuan,
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
            gameId: Arg.Is(query.GameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(game));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlByAsiaGamingAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            gameType: Arg.Is((GameProvidersClientGameType)game.Type),
            gameId: Arg.Is(game.GameId),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            loginRecordId: Arg.Is(query.LoginRecordId),
            platformType: Arg.Is(query.PlatformType),
            language: Arg.Is(query.Language),
            ipAddress: Arg.Is(query.IpAddress),
            backUrl: Arg.Is(query.BackUrl),
            defaultWalletId: Arg.Is(player.DefaultWalletId))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameUrl,
            fakeVerification,
            fakePlayer);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerGameUrlAsync(
               player: Arg.Is(player),
               game: Arg.Is(game),
               cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Get_BetConstruct_Game_Url()
    {
        var fakeGame = Substitute.For<IGame>();
        var fakeGameUrl = Substitute.For<IGameUrl>();
        var fakeVerification = Substitute.For<IVerification>();
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
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakePlayer
            .GetAsync(
            playerCode: Arg.Is(query.PlayerCode),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

        var game = new GameInfo(
            GameId: 1,
            GameCode: "lobby",
            GameProviderId: Guid.NewGuid(),
            GameProviderCode: GameProviderType.BetConstruct,
            GameExternalId: "lobby",
            Type: GameType.Sportsbook,
            PlatformType: GamePlatformType.DesktopAndMobile,
            Name: GameProviderType.Kaiyuan,
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
            gameId: Arg.Is(query.GameId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(game));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlByBetConstructAsync(
            gameProviderCode: Arg.Is(game.GameProviderCode),
            gameType: Arg.Is((GameProvidersClientGameType)game.Type),
            gameId: Arg.Is(game.GameId),
            brandCode: Arg.Is(player.BrandCode),
            playerCode: Arg.Is(player.PlayerCode),
            currencyCode: Arg.Is(player.DefaultWalletCurrencyCode),
            loginRecordId: Arg.Is(query.LoginRecordId),
            platformType: Arg.Is(query.PlatformType),
            language: Arg.Is(query.Language),
            ipAddress: Arg.Is(query.IpAddress),
            backUrl: Arg.Is(query.BackUrl),
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(game.GameProviderId))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameUrl,
            fakeVerification,
            fakePlayer);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(gameUrl, actual);

        _ = fakeVerification
            .Received(1)
            .VerifyPlayerGameUrlAsync(
            player: Arg.Is(player),
            game: Arg.Is(game),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
