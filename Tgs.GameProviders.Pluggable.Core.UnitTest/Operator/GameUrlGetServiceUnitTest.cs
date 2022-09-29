using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameVerification.Client;
using GamePlatformType = Tgs.GameProviders.QueryServer.Models.GamePlatformType;
using GameType = Tgs.GameProviders.QueryServer.Models.GameType;

namespace Tgs.GameProviders.Pluggable.Core.UnitTest.Operator
{
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
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
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
                GameId: 1000001,
                GameExternalId: "0",
                EndpointPath: null,
                RuleUrl: null,
                GameProviderId: Guid.NewGuid(),
                GameType: GameType.CardGameLobby,
                PlatformType: GamePlatformType.DesktopAndMobile,
                GameCode: "Lobby",
                SubGameProviderCode: GameProviderType.KY);
            _ = fakeGame
                .GetAsync(
                    Arg.Is(query.GameId))
                .Returns(ValueTask.FromResult(game));

            var gameProvider = new GameProviderInfo(
                 game.GameProviderId,
                 GameProviderType.KY);
            _ = fakeGameProvider
                .GetAsync(
                    Arg.Is(game.GameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            var gameUrl = new Uri("https://www.google.com/");
            _ = fakeGameUrl
                .GetGameUrlAsync(
                    Arg.Is(gameProvider.GameProviderCode),
                    Arg.Is((GameProvidersClientGameType)game.GameType),
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
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
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
                GameId: 1000001,
                GameExternalId: "0",
                EndpointPath: null,
                RuleUrl: null,
                GameProviderId: Guid.NewGuid(),
                GameType: GameType.LotteryLobby,
                PlatformType: GamePlatformType.DesktopAndMobile,
                GameCode: "Lobby",
                SubGameProviderCode: GameProviderType.AsiaGaming);
            _ = fakeGame
                .GetAsync(
                    Arg.Is(query.GameId))
                .Returns(ValueTask.FromResult(game));

            var gameProvider = new GameProviderInfo(
                 game.GameProviderId,
                 GameProviderType.AsiaGaming);
            _ = fakeGameProvider
                .GetAsync(
                    Arg.Is(game.GameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            var gameUrl = new Uri("https://www.google.com/");
            _ = fakeGameUrl
                .GetGameUrlByAsiaGamingAsync(
                    Arg.Is(gameProvider.GameProviderCode),
                    Arg.Is((GameProvidersClientGameType)game.GameType),
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
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
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
                GameId: 1000001,
                GameExternalId: "0",
                EndpointPath: null,
                RuleUrl: null,
                GameProviderId: Guid.NewGuid(),
                GameType: GameType.Sportsbook,
                PlatformType: GamePlatformType.DesktopAndMobile,
                GameCode: "Lobby",
                SubGameProviderCode: GameProviderType.BetConstruct);
            _ = fakeGame
                .GetAsync(
                    Arg.Is(query.GameId))
                .Returns(ValueTask.FromResult(game));

            var gameProvider = new GameProviderInfo(
                 game.GameProviderId,
                 GameProviderType.BetConstruct);
            _ = fakeGameProvider
                .GetAsync(
                    Arg.Is(game.GameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            var gameUrl = new Uri("https://www.google.com/");
            _ = fakeGameUrl
                .GetGameUrlByBetConstructAsync(
                    Arg.Is(gameProvider.GameProviderCode),
                    Arg.Is((GameProvidersClientGameType)game.GameType),
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
}
