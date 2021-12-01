using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Services;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

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
               GameProviderType.KY,
               string.Empty
              );
            var gameModel = new GameModel(
               1000032,
               GameStatus.Active,
               "0",
               new("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
               GameType.CardGameLobby,
               GamePlatformType.DesktopAndMobile,
               false,
               "Lobby",
               "Lobby",
               "KY"
              );

            var gameProviderCurrencyModel = new GameProviderCurrencyModel(
                gameProviderModel.GameProviderId,
                playerModel.DefaultWalletCurrencyCode,
                "RMB");

            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakeLicenseeRepository = Substitute.For<ILicenseeRepository>();
            var faceLogger = NullLogger<PlayerVerifyService>.Instance;
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeResellerRepository = Substitute.For<IResellerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                    _ = fakeGameProvidersClient
                     .GameUrlAsync(gameProviderModel.GameProviderCode, gameModel.GameType, gameModel.GameId, playerModel.BrandCode, playerModel.PlayerCode, playerModel.DefaultWalletCurrencyCode, loginRecordId, platformType, language, ipAddress, backUrl)
                     .Returns(ValueTask.FromResult<Uri?>(gameurl));

                    _ = fakeGameProvidersClient
                     .GameUrlAsync(
                        Arg.Is(gameProviderModel.GameProviderCode),
                        Arg.Is(gameModel.GameType),
                        Arg.Is(gameModel.GameId),
                        Arg.Is(playerModel.BrandCode),
                        Arg.Is(playerModel.PlayerCode),
                        Arg.Is(playerModel.DefaultWalletCurrencyCode),
                        Arg.Is(loginRecordId),
                        Arg.Is(platformType),
                        Arg.Is(language),
                        Arg.Is(ipAddress),
                        Arg.Is(backUrl)
                        )
                     .Returns(ValueTask.FromResult<Uri?>(gameurl));
                    services.AddSingleton(fakeGameProvidersClient);
                    services.AddSingleton(fakeGameProvidersClient);
                    services.AddSingleton(fakeBrandRepository);
                    services.AddSingleton(fakeGameProviderRepository);
                    services.AddSingleton(fakeGameRepository);
                    services.AddSingleton(fakeLicenseeRepository);
                    services.AddSingleton(faceLogger);
                    services.AddSingleton(fakePlayerRepository);
                    services.AddSingleton(fakeResellerRepository);
                    services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            _ = fakePlayerRepository
                .GetAsync(playerCode)
                .Returns(ValueTask.FromResult(playerModel));

            _ = fakeGameProviderRepository
                .GetAsync(gameModel.GameProviderId)
                .Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakeGameRepository.GetAsync(gameId)
                .Returns(ValueTask.FromResult(gameModel));

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(
                Arg.Is(playerCode)).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository.GetAsync(
                Arg.Is(playerCode)).Returns(ValueTask.FromResult(playerModel));

            _ = fakePlayerVerifyService.IsPlayerGameActiveAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameModel.GameId)).Returns(ValueTask.FromResult(true));

            _ = fakeGameProviderRepository.GetCurrencyAsync(
                Arg.Is(gameProviderCurrencyModel.GameProviderId),
                Arg.Is(gameProviderCurrencyModel.CurrencyCode))
                .Returns(ValueTask.FromResult<GameProviderCurrencyModel?>(gameProviderCurrencyModel));

            _ = fakeBrandRepository.GetCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                )
                .Returns(ValueTask.FromResult<string?>(playerModel.DefaultWalletCurrencyCode));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameModel.GameProviderId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                ).Returns(ValueTask.FromResult(true));

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
