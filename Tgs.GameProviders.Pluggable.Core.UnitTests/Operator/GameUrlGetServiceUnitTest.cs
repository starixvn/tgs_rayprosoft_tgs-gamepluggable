using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class GameUrlGetServiceUnitTest
    {
        [Test]
        public async Task Game_Url()
        {
            var faceLogger = NullLogger<GameUrlGetService>.Instance;
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var fakeService = new GameUrlGetService(
                  fakeGameProviderRepository,
                  fakeGameProvidersClient,
                  fakeGameRepository,
                  faceLogger,
                  fakePlayerRepository,
                  fakePlayerVerifyService,
                  fakeBrandRepository);

            var query = new GameUrlQuery(
                 "4ucmqplxb029",
                 Guid.NewGuid(),
                 PlatformType.Desktop,
                 1000001,
                 "zh-CN",
                 "127.0.0.1",
                 new("https://dev-t1.rpgp.cc/"));

            var playerModel = new PlayerModel(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled
                );

            var gameModel = new GameModel(
                1000001,
                 GameStatus.Active,
                 "0",
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 GameType.LiveDealerLobby,
                 GamePlatformType.DesktopAndMobile,
                 false,
                 "Lobby",
                 "Lobby",
                 "ky"
                );

            var gameProviderModel = new GameProviderModel(
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 GameProviderStatus.Active,
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

            _ = fakePlayerVerifyService
                .IsPlayerEnableAsync(
                Arg.Is(query.PlayerCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakePlayerVerifyService
                .IsPlayerGameActiveAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(true));

            _ = fakeGameRepository.GetAsync(
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(gameModel));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository
                .GetAsync(Arg.Is(query.PlayerCode))
                .Returns(ValueTask.FromResult(playerModel));

            _ = fakeGameProviderRepository
                .GetAsync(Arg.Is(gameModel.GameProviderId))
                .Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakeGameProvidersClient.GameUrlAsync(
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(gameModel.GameType),
                Arg.Is(query.GameId),
                Arg.Is(playerModel.BrandCode),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl))
                .Returns(ValueTask.FromResult(new Uri("https://www.google.com/")));

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(new Uri("https://www.google.com/"), actual);
        }

        [Test]
        public async Task Get_BetConstruct_Game_Url()
        {
            var faceLogger = NullLogger<GameUrlGetService>.Instance;
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();
            var fakeBrandRepository = Substitute.For<IBrandRepository>();

            var fakeService = new GameUrlGetService(
                  fakeGameProviderRepository,
                  fakeGameProvidersClient,
                  fakeGameRepository,
                  faceLogger,
                  fakePlayerRepository,
                  fakePlayerVerifyService,
                  fakeBrandRepository);

            var query = new GameUrlQuery(
                 "4ucmqplxb029",
                 Guid.NewGuid(),
                 PlatformType.Desktop,
                 2100001,
                 "zh-CN",
                 "127.0.0.1",
                 new("https://dev-t1.rpgp.cc/"));

            var playerModel = new PlayerModel(
                Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                "TGS",
                Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                "TGS",
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                "4ucmqplxb029",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY",
                PlayerStatus.Enabled
                );

            var gameModel = new GameModel(
                1000001,
                 GameStatus.Active,
                 "0",
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 GameType.Sportsbook,
                 GamePlatformType.DesktopAndMobile,
                 false,
                 "Lobby",
                 "Lobby",
                 "bc"
                );

            var gameProviderModel = new GameProviderModel(
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

            _ = fakePlayerVerifyService
                .IsPlayerEnableAsync(
                Arg.Is(query.PlayerCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakePlayerVerifyService
                .IsPlayerGameActiveAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(true));

            _ = fakeGameRepository.GetAsync(
                Arg.Is(query.GameId)).Returns(ValueTask.FromResult(gameModel));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                ).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository
                .GetAsync(Arg.Is(query.PlayerCode))
                .Returns(ValueTask.FromResult(playerModel));

            _ = fakeGameProviderRepository
                .GetAsync(Arg.Is(gameModel.GameProviderId))
                .Returns(ValueTask.FromResult(gameProviderModel));

            var gameProviderConfigurationId = Guid.Parse("575a70ec-1004-b324-4444-2ade0261a0b2");

            _ = fakeBrandRepository
                    .GetBrandGameProviderConfigurationAsync(playerModel.BrandId, gameProviderModel.GameProviderId)
                    .Returns(ValueTask.FromResult(new BrandGameProviderModel() { GameProviderConfigurationId = gameProviderConfigurationId }));

            _ = fakeGameProviderRepository
                .GetGameProviderConfigurationAsync(gameModel.GameProviderId, gameProviderConfigurationId)
                .Returns(ValueTask.FromResult(new GameProviderConfigModel() { DesktopEndpoint = "https://desktop.com", MobileEndpoint = "https://mobile.com" }));

            var lobbyDomain = "https://desktop.com";

            _ = fakeGameProvidersClient.GameBetConstructUrlAsync(
                Arg.Is(gameProviderModel.GameProviderCode),
                Arg.Is(gameModel.GameType),
                Arg.Is(query.GameId),
                Arg.Is(playerModel.BrandCode),
                Arg.Is(playerModel.PlayerCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode),
                Arg.Is(query.LoginRecordId),
                Arg.Is(query.PlatformType),
                Arg.Is(query.Language),
                Arg.Is(query.IpAddress),
                Arg.Is(query.BackUrl),
                Arg.Is(lobbyDomain))
                .Returns(ValueTask.FromResult(new Uri("https://desktop.com")));

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(new Uri("https://desktop.com"), actual);
        }
    }
}