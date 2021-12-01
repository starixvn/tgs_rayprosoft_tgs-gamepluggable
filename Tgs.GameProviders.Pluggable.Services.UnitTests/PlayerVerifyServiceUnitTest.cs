using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Services.UnitTests
{
    [TestFixture]
    public class PlayerVerifyServiceUnitTest
    {
        [Test]
        public async Task IsGameEnableAsync()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakeLicenseeRepository = Substitute.For<ILicenseeRepository>();
            var faceLogger = NullLogger<PlayerVerifyService>.Instance;
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeResellerRepository = Substitute.For<IResellerRepository>();

            var service = new PlayerVerifyService(
                fakeBrandRepository,
                fakeGameProviderRepository,
                fakeGameRepository,
                fakeLicenseeRepository,
                fakePlayerRepository,
                faceLogger,
                fakeResellerRepository
                );

            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
            var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");
            var gameId = 1000031;

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

            var gameModel = new GameModel()
            {
                GameId = 1000031,
                Status = GameStatus.Active,
                GameExternalId = "0",
                GameProviderId = gameProviderId,
                GameType = GameType.LiveDealerLobby,
                PlatformType = GamePlatformType.DesktopAndMobile,
                SubGame = false,
                GameCode = "Lobby",
                Name = "Lobby",
                SubGameProviderCode = "KY",
            };

            _ = fakeGameRepository
                .GetAsync(gameId)
                .Returns(ValueTask.FromResult(gameModel));

            _ = fakeGameProviderRepository
                .GetAsync(gameProviderId)
                .Returns(ValueTask.FromResult(gameProviderModel));

            _ = await service.IsPlayerGameActiveAsync(brandId, gameId).ConfigureAwait(false);

            _ = fakeBrandRepository
                .Received(1)
                .GetGameProviderStatusAsync(brandId, gameModel.GameProviderId);
            _ = fakeBrandRepository
                .Received(1)
                .GetGameStatusAsync(brandId, gameModel.GameId);
        }

        [Test]
        public async Task IsPlayerEnableAsync()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakeLicenseeRepository = Substitute.For<ILicenseeRepository>();
            var faceLogger = NullLogger<PlayerVerifyService>.Instance;
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeResellerRepository = Substitute.For<IResellerRepository>();

            var service = new PlayerVerifyService(
                fakeBrandRepository,
                fakeGameProviderRepository,
                fakeGameRepository,
                fakeLicenseeRepository,
                fakePlayerRepository,
                faceLogger,
                fakeResellerRepository
                );

            var playerCode = "4ucmqplxb029";
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

            _ = fakePlayerRepository
                .GetAsync(playerCode)
                .Returns(ValueTask.FromResult<PlayerModel>(playerModel));

            _ = await service.IsPlayerEnableAsync(playerCode).ConfigureAwait(false);

            _ = fakeResellerRepository
                .Received(1)
                .GetStatusAsync(playerModel.ResellerId);

            _ = fakeLicenseeRepository
                .Received(1)
                .GetStatusAsync(playerModel.LicenseeId);

            _ = fakeBrandRepository
                .Received(1)
                .GetStatusAsync(playerModel.BrandId);
        }

        [Test]
        public async Task IsPlayerGameProviderActiveAsync()
        {
            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakeLicenseeRepository = Substitute.For<ILicenseeRepository>();
            var faceLogger = NullLogger<PlayerVerifyService>.Instance;
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeResellerRepository = Substitute.For<IResellerRepository>();

            var service = new PlayerVerifyService(
                fakeBrandRepository,
                fakeGameProviderRepository,
                fakeGameRepository,
                fakeLicenseeRepository,
                fakePlayerRepository,
                faceLogger,
                fakeResellerRepository
                );

            var gameProviderCode = "KY";
            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
            var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");

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

            _ = fakeGameProviderRepository
                .GetAsync(gameProviderCode)
                .Returns(ValueTask.FromResult(gameProviderModel));

            _ = await service.IsPlayerGameProviderActiveAsync(brandId, gameProviderCode).ConfigureAwait(false);

            _ = fakeBrandRepository
                .Received(1)
                .GetGameProviderStatusAsync(brandId, gameProviderId);
        }
    }
}
