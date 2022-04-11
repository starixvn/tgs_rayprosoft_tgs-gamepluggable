using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.CashService.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameVerifys.Client;
using GamePlatformType = Tgs.GameProviders.QueryServer.Models.GamePlatformType;
using GameProviderStatus = Tgs.GameProviders.QueryServer.Models.GameProviderStatus;
using GameStatus = Tgs.GameProviders.QueryServer.Models.GameStatus;
using GameType = Tgs.GameProviders.QueryServer.Models.GameType;
using PlayerStatus = Tgs.GameProviders.QueryServer.Models.PlayerStatus;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class GameUrlGetServiceUnitTest
    {
        [Test]
        public async Task Game_Url()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeGame = Substitute.For<IGameService>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakeGameVerifysClient = Substitute.For<IGameVerifysClient>();

            var gameUrl = new Uri("https://www.google.com/");

            var query = new GameUrlQuery(
                 "4ucmqplxb029",
                 Guid.NewGuid(),
                 GameProvidersClientPlatformType.Desktop,
                 1000001,
                 "zh-CN",
                 "127.0.0.1",
                 new("https://dev-t1.rpgp.cc/"));

            var player = new PlayerInfo(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled
                );

            var game = new GameInfo(
                1000001,
                 GameStatus.Active,
                 "0",
                 null,
                 null,
                 null,
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 GameType.LiveDealerLobby,
                 GamePlatformType.DesktopAndMobile,
                 false,
                 false,
                 false,
                 true,
                 null,
                 "Lobby",
                 "Lobby",
                 null,
                 null
                );

            var gameProvider = new GameProviderInfo(
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                QueryServer.Models.GameProviderStatus.Active,
                 string.Empty,
                 true,
                 true,
                 false,
                 0,
                 0,
                 (int)GameProviderUnique.KY,
                 true,
                 GameProviderType.KY,
                 GameProviderUnique.KY.ToString(),
                 string.Empty
                 );

            _ = fakePlayerService
                .GetAsync(Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameVerifysClient
                .VerifyPlayerGameUrlAsync(query.PlayerCode, query.GameId).Returns(ValueTask.FromResult(true));

            _ = fakeGame.GetAsync(
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(game));

            _ = fakeGameProvider
                .GetAsync(Arg.Is(game.GameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            _ = fakeGameProvidersClient.GameUrlAsync(
                gameProvider.GameProviderCode,
                (GameProvidersClientGameType)game.GameType,
                Arg.Is(query.GameId),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl)).Returns(ValueTask.FromResult(gameUrl));

            var sut = new GameUrlGetService(
                fakeBrand,
                fakeCashServiceClient,
                fakeGameProvidersClient,
                fakeGameProvider,
                fakeGame,
                fakePlayerService,
                fakeGameVerifysClient
                );

            var actual = await sut.GetAsync(query).ConfigureAwait(false);

            Assert.AreEqual(gameUrl, actual);
        }

        [Test]
        public async Task Get_AsiaGaming_Game_Url()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeGame = Substitute.For<IGameService>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakeGameVerifysClient = Substitute.For<IGameVerifysClient>();

            var playerBalance = 1000m;
            var gameUrl = new Uri("http://www.google.com");

            var player = new PlayerInfo(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                (QueryServer.Models.PlayerStatus)PlayerStatus.Enabled);

            var gameProvider = new GameProviderInfo(
                 Guid.Parse("629101fc-3b1c-4719-881b-dcf3c7263c15"),
                 GameProviderStatus.Active,
                 string.Empty,
                 true,
                 true,
                 false,
                 0,
                 0,
                 (int)GameProviderUnique.AsiaGaming,
                 true,
                 GameProviderType.AsiaGaming,
                 nameof(GameProviderType.AsiaGaming),
                 string.Empty
                 );

            var game = new GameInfo(
                400001,
                QueryServer.Models.GameStatus.Active,
                "0",
                null,
                null,
                null,
                gameProvider.GameProviderId,
                QueryServer.Models.GameType.Sportsbook,
                QueryServer.Models.GamePlatformType.DesktopAndMobile,
                false,
                false,
                false,
                true,
                null,
                "Lobby",
                "Lobby",
                null,
                null);

            var query = new GameUrlQuery(
                player.PlayerCode,
                Guid.NewGuid(),
                GameProvidersClientPlatformType.Desktop,
                game.GameId,
                "zh-CN",
                "127.0.0.1",
                new("https://dev-t1.rpgp.cc/"));

            _ = fakePlayerService
                .GetAsync(Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameVerifysClient
                .VerifyPlayerGameUrlAsync(query.PlayerCode, query.GameId).Returns(ValueTask.FromResult(true));

            _ = fakeGame.GetAsync(
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(game));

            _ = fakeGameProvider
                .GetAsync(Arg.Is(game.GameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            _ = fakeCashServiceClient
                .GetBalanceAsync(
                    Arg.Is(player.DefaultWalletId))
                .Returns(ValueTask.FromResult(playerBalance));

            _ = fakeGameProvidersClient.GameUrlByAsiaGamingAsync(
                gameProvider.GameProviderCode,
                (GameProvidersClientGameType)game.GameType,
                Arg.Is(query.GameId),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl),
                playerBalance).Returns(ValueTask.FromResult(gameUrl));

            var sut = new GameUrlGetService(
                fakeBrand,
                fakeCashServiceClient,
                fakeGameProvidersClient,
                fakeGameProvider,
                fakeGame,
                fakePlayerService,
                fakeGameVerifysClient
                );

            var actual = await sut.GetAsync(query).ConfigureAwait(false);

            Assert.AreEqual(gameUrl, actual);
        }

        [Test]
        public async Task Get_BetConstruct_Game_Url()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeGame = Substitute.For<IGameService>();
            var fakePlayerService = Substitute.For<IPlayerService>();
            var fakeGameVerifysClient = Substitute.For<IGameVerifysClient>();

            var lobbyDomain = "https://desktop.com";
            var gameUrl = new Uri("https://www.google.com");

            var query = new GameUrlQuery(
                 "4ucmqplxb029",
                 Guid.NewGuid(),
                 GameProvidersClientPlatformType.Desktop,
                 2100001,
                 "zh-CN",
                 "127.0.0.1",
                 new("https://dev-t1.rpgp.cc/"));

            var player = new PlayerInfo(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled
                );

            var gameProvider = new GameProviderInfo(
                 Guid.Parse("39247048-e631-0aaa-4553-865bb86d27ec"),
                 GameProviderStatus.Active,
                 string.Empty,
                 true,
                 true,
                 false,
                 0,
                 0,
                 (int)GameProviderUnique.BetConstruct,
                 true,
                 GameProviderType.BetConstruct,
                 GameProviderUnique.BetConstruct.ToString(),
                 string.Empty
                 );

            var game = new GameInfo(
                1000001,
                GameStatus.Active,
                "0",
                null,
                null,
                null,
                gameProvider.GameProviderId,
                GameType.Sportsbook,
                GamePlatformType.DesktopAndMobile,
                false,
                true,
                true,
                true,
                null,
                "BC",
                "Lobby",
                null,
                null);

            _ = fakePlayerService
    .GetAsync(Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameVerifysClient
                .VerifyPlayerGameUrlAsync(query.PlayerCode, query.GameId).Returns(ValueTask.FromResult(true));

            _ = fakeGame.GetAsync(
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(game));

            _ = fakeGameProvider
                .GetAsync(Arg.Is(game.GameProviderId))
                .Returns(ValueTask.FromResult(gameProvider));

            var gameProviderConfigurationId = Guid.Parse("575a70ec-1004-b324-4444-2ade0261a0b2");

            _ = fakeBrand
                    .GetBrandGameProviderConfigurationAsync(player.BrandId, gameProvider.GameProviderId)
                    .Returns(ValueTask.FromResult(new BrandGameProviderInfo(
                        GameProviderId: gameProvider.GameProviderId,
                        GameProviderConfigurationId: gameProviderConfigurationId,
                        DefaultLanguageCode: "EN",
                        DesktopQueryString: lobbyDomain,
                        MobileQueryString: lobbyDomain
                        )));

            _ = fakeGameProvider
                .GetConfigurationAsync(gameProviderConfigurationId)
                .Returns(ValueTask.FromResult(new GameProviderConfigInfo(
                    gameProviderConfigurationId,
                    gameProvider.Name,
                    string.Empty,
                    Guid.NewGuid().ToString("N"),
                    Guid.NewGuid().ToString("N"),
                    GameProviderConfigStatus.Active,
                    gameProvider.GameProviderId,
                    DesktopEndpoint: "https://desktop.com",
                    MobileEndpoint: "https://mobile.com",
                    ConfigurationData: string.Empty,
                    IsDefault: false)));

            _ = fakeGameProvidersClient.GameUrlByBetConstructAsync(
                gameProvider.GameProviderCode,
                (GameProvidersClientGameType)game.GameType,
                Arg.Is(query.GameId),
                Arg.Is(player.BrandCode),
                Arg.Is(player.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl),
                lobbyDomain)
                .Returns(ValueTask.FromResult(gameUrl));

            var sut = new GameUrlGetService(
                fakeBrand,
                fakeCashServiceClient,
                fakeGameProvidersClient,
                fakeGameProvider,
                fakeGame,
                fakePlayerService,
                fakeGameVerifysClient
                );

            var actual = await sut.GetAsync(query).ConfigureAwait(false);

            Assert.AreEqual(gameUrl, actual);
        }
    }
}
