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
                playerCode: Arg.Is(query.PlayerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(player));

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
                gameId: Arg.Is(query.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(game));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlAsync(
                Arg.Is(game.GameProviderCode),
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
            fakeGameUrl,
            fakeVerification,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

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
                gameId: Arg.Is(query.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(game));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlByAsiaGamingAsync(
                Arg.Is(game.GameProviderCode),
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
            fakeGameUrl,
            fakeVerification,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

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
                gameId: Arg.Is(query.GameId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult(game));

        var gameUrl = new Uri("https://www.google.com/");
        _ = fakeGameUrl
            .GetGameUrlByBetConstructAsync(
                Arg.Is(game.GameProviderCode),
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
                Arg.Is(game.GameProviderId))
            .Returns(ValueTask.FromResult(gameUrl));

        var sut = new GameUrlGetService(
            fakeGame,
            fakeGameUrl,
            fakeVerification,
            fakePlayer);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

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
