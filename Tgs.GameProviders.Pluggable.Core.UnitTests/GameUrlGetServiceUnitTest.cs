using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core.ApplicationServices;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests
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

            var fakeService = new GameUrlGetService(
                  fakeGameProviderRepository,
                  fakeGameProvidersClient,
                  fakeGameRepository,
                  faceLogger,
                  fakePlayerRepository,
                  fakePlayerVerifyService);

            var query = new GameUrlQuery()
            {
                SubGameProviderCode = "ky",

                PlayerCode = "4ucmqplxb029",
                LoginRecordId = Guid.NewGuid(),
                PlatformType = PlatformType.Desktop,
                GameId = 1000001,
                Language = "zh-CN",
                IpAddress = "127.0.0.1",
                BackUrl = new("https://dev-t1.rpgp.cc/")
            };

            var playerModel = new PlayerModel()
            {
                ResellerId = Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
                LicenseeId = Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
                LicenseeCode = "TGS",
                BrandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                BrandCode = "GPI",
                PlayerCode = "4ucmqplxb029",
                Username = "4ucmqplxb029",
                RealName = "4ucmqplxb029",
                DefaultWalletId = Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                DefaultWalletCurrencyCode = "CNY",
                Status = PlayerStatus.Enabled
            };

            var gameModel = new GameModel()
            {
                GameId = 1000001,
                Status = GameStatus.Active,
                GameExternalId = "0",
                GameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                GameType = GameType.LiveDealerLobby,
                PlatformType = GamePlatformType.DesktopAndMobile,
                SubGame = false,
                GameCode = "Lobby",
                Name = "Lobby",
                SubGameProviderCode = "ky"
            };

            var gameProviderModel = new GameProviderModel()
            {
                GameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                Status = GameProviderStatus.Active,
                AutoCloseRound = true,
                SupportTestPlayer = true,
                AllowKickPlayer = false,
                PlaceBetTimeout = 0,
                SettleBetTimeout = 0,
                UniqueId = 10,
                TransferWallet = true,
                GameProviderCode = "KY",
                Name = "KY"
            };

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(query.PlayerCode).Returns(ValueTask.FromResult(true));
            _ = fakePlayerVerifyService.IsPlayerGameActiveAsync(playerModel.BrandId, query.GameId).Returns(ValueTask.FromResult(true));
            _ = fakeGameRepository.GetAsync(query.GameId).Returns(ValueTask.FromResult(gameModel));
            _ = fakePlayerRepository.GetAsync(query.PlayerCode).Returns(ValueTask.FromResult(playerModel));
            _ = fakeGameProviderRepository.GetAsync(gameModel.GameProviderId).Returns(ValueTask.FromResult(gameProviderModel));
            _ = fakeGameProvidersClient.GameUrlAsync(
                gameProviderModel.GameProviderCode,
                gameModel.GameType,
                query.GameId,
                playerModel.BrandCode,
                playerModel.PlayerCode,
                playerModel.DefaultWalletCurrencyCode,
                query.LoginRecordId,
                query.PlatformType,
                query.Language,
                query.IpAddress,
                query.BackUrl)
                .Returns(ValueTask.FromResult(new Uri("https://www.google.com/")));

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(new Uri("https://www.google.com/"), actual);
        }
    }
}
