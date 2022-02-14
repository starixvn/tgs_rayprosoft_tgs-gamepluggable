using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Domains;
using GamePlatformType = Tgs.GameProviders.QueryServer.Models.GamePlatformType;
using GameProviderStatus = Tgs.GameProviders.QueryServer.Models.GameProviderStatus;
using GameStatus = Tgs.GameProviders.QueryServer.Models.GameStatus;
using GameType = Tgs.GameProviders.QueryServer.Models.GameType;
using PlayerStatus = Tgs.GameProviders.QueryServer.Models.PlayerStatus;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests
{
    [TestFixture]
    public class PlayerVerifyServiceUnitTest
    {
        [Test]
        public async Task IsGameEnableAsync()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeLicensee = Substitute.For<ILicenseeService>();
            var fakeReseller = Substitute.For<IResellerService>();
            var playerVerify = new PlayerVerifyHanlde(
                fakeBrand,
                fakeGameProvider,
                fakeLicensee,
                fakeReseller);

            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
            var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");
            var gameId = 1000031;

            var gameProvider = new GameProviderInfo(
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

            var game = new GameInfo(
                gameId,
                GameStatus.Active,
                "test",
                string.Empty,
                string.Empty,
                string.Empty,
                gameProviderId,
                GameType.CardGame,
                GamePlatformType.DesktopAndMobile,
                true,
                false,
                false,
                false,
                null,
                "test",
                "test",
                null,
                "ky");

            _ = fakeGameProvider
                .GetAsync(gameProviderId)
                .Returns(ValueTask.FromResult(gameProvider));

            _ = await playerVerify.IsPlayerGameActiveAsync(brandId, game).ConfigureAwait(false);

            _ = fakeBrand
                .Received(1)
                .GetGameProviderStatusAsync(brandId, game.GameProviderId);
            _ = fakeBrand
                .Received(1)
                .GetGameStatusAsync(brandId, game.GameId);
        }

        [Test]
        public async Task IsPlayerEnableAsync()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeLicensee = Substitute.For<ILicenseeService>();
            var fakeReseller = Substitute.For<IResellerService>();
            var playerVerify = new PlayerVerifyHanlde(
                fakeBrand,
                fakeGameProvider,
                fakeLicensee,
                fakeReseller);

            var resellerId = Guid.NewGuid();
            var resellerCode = "TGS";
            var licenseeId = Guid.NewGuid();
            var licenseeCode = "TGS";
            var brandId = Guid.NewGuid();
            var brandCode = "GPI";
            var playerCode = "4ucmqplxb029";
            var player = new PlayerInfo(
                resellerId,
                resellerCode,
                licenseeId,
                licenseeCode,
                brandId,
                brandCode,
                playerCode,
                playerCode,
                Guid.NewGuid(),
                "CNY",
                PlayerStatus.Enabled);

            _ = await playerVerify.IsPlayerEnableAsync(player).ConfigureAwait(false);

            _ = fakeBrand
                .Received(1)
                .GetStatusAsync(player.BrandId);

            _ = fakeLicensee
                .Received(1)
                .GetStatusAsync(player.LicenseeId);

            _ = fakeReseller
                .Received(1)
                .GetStatusAsync(player.ResellerId);
        }

        [Test]
        public async Task IsPlayerGameProviderActiveAsync()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeLicensee = Substitute.For<ILicenseeService>();
            var fakeReseller = Substitute.For<IResellerService>();
            var playerVerify = new PlayerVerifyHanlde(
                fakeBrand,
                fakeGameProvider,
                fakeLicensee,
                fakeReseller);

            var gameProviderCode = "KY";
            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
            var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");

            var gameProvider = new GameProviderInfo(
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

            _ = await playerVerify.IsPlayerGameProviderActiveAsync(brandId, gameProvider).ConfigureAwait(false);

            _ = fakeBrand
                .Received(1)
                .GetGameProviderStatusAsync(brandId, gameProviderId);
        }

        [Test]
        public async Task IsPlayerGameProviderIdCurrencyAsync()
        {
            var fakeBrand = Substitute.For<IBrandService>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakeLicensee = Substitute.For<ILicenseeService>();
            var fakeReseller = Substitute.For<IResellerService>();
            var playerVerify = new PlayerVerifyHanlde(
                fakeBrand,
                fakeGameProvider,
                fakeLicensee,
                fakeReseller);

            var currencyCode = "CNY";
            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
            var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");

            _ = fakeGameProvider
                .GetCurrencyCodeAsync(gameProviderId, currencyCode)
                .Returns(ValueTask.FromResult("RMB"));

            _ = fakeBrand
                .GetCurrencyCodeAsync(brandId, currencyCode)
                .Returns(ValueTask.FromResult(currencyCode));

            var actual = await playerVerify.IsPlayerGameProviderCurrencyAsync(
                brandId,
                gameProviderId,
                currencyCode).ConfigureAwait(false);
            Assert.NotNull(actual);
            Assert.AreEqual(true, actual);
        }
    }
}
