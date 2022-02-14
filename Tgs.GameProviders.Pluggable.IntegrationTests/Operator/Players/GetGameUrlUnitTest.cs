using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.QueryServer.Models;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;
using GamePlatformType = Tgs.GameProviders.QueryServer.Models.GamePlatformType;
using GameProviderStatus = Tgs.GameProviders.QueryServer.Models.GameProviderStatus;
using GameStatus = Tgs.GameProviders.QueryServer.Models.GameStatus;
using GameType = Tgs.GameProviders.QueryServer.Models.GameType;
using PlayerStatus = Tgs.GameProviders.QueryServer.Models.PlayerStatus;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Operator.Players
{
    [TestClass]
    public class GetGameUrlUnitTest
    {
        [TestMethod]
        public async Task GetGameUrlAsync()
        {
            string playerCode = "4ucmqplxb029";
            Guid loginRecordId = Guid.NewGuid();
            var platformType = PlatformType.Desktop;
            bool isMobile = platformType == PlatformType.Mobile;
            int gameId = 1000032;
            string language = "zh-CN";
            string ipAddress = "127.0.0.1";
            Uri backUrl = new("https://dev-t1.rpgp.cc");
            Uri gameurl = new("https://www.google.com/");

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
               GameProviderType.KY,
               string.Empty
              );
            var game = new GameInfo(
               1000032,
               GameStatus.Active,
               "0",
               null,
               null,
               null,
               new("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
               GameType.CardGameLobby,
               GamePlatformType.DesktopAndMobile,
               false,
               false,
               false,
               true,
               null,
               "Lobby",
               "Lobby",
               null,
               null);

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                    _ = fakeGameProvidersClient
                     .GameUrlAsync(
                        gameProvider.GameProviderCode,
                        (TGS.GameProviders.Domains.GameType)game.GameType,
                        game.GameId,
                        player.BrandCode,
                        player.PlayerCode,
                        player.DefaultWalletCurrencyCode, loginRecordId, platformType, language, ipAddress, backUrl)
                     .Returns(ValueTask.FromResult<Uri?>(gameurl));

                    services.AddSingleton(fakeGameProvidersClient);

                    var fakePlayer = Substitute.For<IPlayerService>();

                    _ = fakePlayer
                        .GetAsync(playerCode)
                        .Returns(ValueTask.FromResult(player));

                    services.AddSingleton(fakePlayer);

                    var fakeGame = Substitute.For<IGameService>();

                    _ = fakeGame
                        .GetAsync(gameId)
                        .Returns(ValueTask.FromResult(game));

                    services.AddSingleton(fakeGame);

                    var fakeBrand = Substitute.For<IBrandService>();

                    _ = fakeBrand
                        .GetStatusAsync(player.BrandId)
                        .Returns(ValueTask.FromResult(BrandStatus.Active));
                    _ = fakeBrand.GetCurrencyCodeAsync(
                        Arg.Is(player.BrandId),
                        Arg.Is(player.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult<string?>(player.DefaultWalletCurrencyCode));

                    services.AddSingleton(fakeBrand);

                    var fakeLicensee = Substitute.For<ILicenseeService>();

                    _ = fakeLicensee
                        .GetStatusAsync(player.LicenseeId)
                        .Returns(ValueTask.FromResult(LicenseeStatus.Active));

                    services.AddSingleton(fakeLicensee);

                    var fakeReseller = Substitute.For<IResellerService>();

                    _ = fakeReseller
                        .GetStatusAsync(player.ResellerId)
                        .Returns(ValueTask.FromResult(ResellerStatus.Active));

                    services.AddSingleton(fakeReseller);

                    var fakeGameProvider = Substitute.For<IGameProviderService>();

                    _ = fakeGameProvider.GetAsync(
                        Arg.Is(gameProvider.GameProviderId)).Returns(ValueTask.FromResult(gameProvider));

                    _ = fakeGameProvider.GetCurrencyCodeAsync(
                        Arg.Is(gameProvider.GameProviderId),
                        Arg.Is(player.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult<string?>(player.DefaultWalletCurrencyCode));

                    services.AddSingleton(fakeGameProvider);

                    var fakePlayerVerify = Substitute.For<IPlayerVerifyHanlde>();
                    _ = fakePlayerVerify.IsPlayerEnableAsync(
                        player).Returns(ValueTask.FromResult(true));

                    _ = fakePlayerVerify.IsPlayerGameActiveAsync(
                        player.BrandId,
                        game).Returns(ValueTask.FromResult(true));

                    _ = fakePlayerVerify.IsPlayerGameProviderCurrencyAsync(
                        player.BrandId,
                        gameProvider.GameProviderId,
                        player.DefaultWalletCurrencyCode).Returns(ValueTask.FromResult(true));

                    services.AddSingleton(fakePlayerVerify);

                    services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            var actual = await sut.GetGameUrlAsync(
                playerCode,
                loginRecordId,
                isMobile,
                gameId,
                language,
                ipAddress,
                backUrl).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameurl, actual);
        }
    }
}
