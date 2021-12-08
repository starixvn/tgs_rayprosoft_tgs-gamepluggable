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
    public class GetBalanceUnitTest
    {
        [TestMethod]
        public async Task GetBalanceAsync()
        {
            var playerCode = "4ucmqplxb029";
            var gameProviderCode = "VR";
            decimal? gameProviderBalance = 0.88m;
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
               Guid.Parse("ae5533bd-35d5-5478-19d6-233ed44f7c9d"),
               GameProviderStatus.Active,
               string.Empty,
               true,
               true,
               false,
               0,
               0,
               (int)GameProviderUnique.VR,
               true,
               GameProviderType.VR,
               GameProviderUnique.VR.ToString(),
               string.Empty
              );

            var gameProviderCurrencyModel = new GameProviderCurrencyModel(
                gameProviderModel.GameProviderId,
                playerModel.DefaultWalletCurrencyCode,
                "RMB"
                );
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
                     .PlayerBalanceAsync(gameProviderCode, playerCode)
                     .Returns(ValueTask.FromResult(gameProviderBalance));

                    _ = fakeGameProvidersClient
                     .PlayerBalanceAsync(Arg.Is(gameProviderCode), Arg.Is(playerCode))
                     .Returns(ValueTask.FromResult(gameProviderBalance));
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
                .GetAsync(gameProviderCode)
                .Returns(ValueTask.FromResult(gameProviderModel));

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(
                       Arg.Is(playerCode)).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository.GetAsync(
                Arg.Is(playerCode)).Returns(ValueTask.FromResult(playerModel));

            _ = fakePlayerVerifyService.IsPlayerGameProviderActiveAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameProviderCode)).Returns(ValueTask.FromResult(true));

            _ = fakeGameProviderRepository.GetCurrencyAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(gameProviderCurrencyModel.CurrencyCode))
                .Returns(ValueTask.FromResult<GameProviderCurrencyModel?>(gameProviderCurrencyModel));

            _ = fakeBrandRepository.GetCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                )
                .Returns(ValueTask.FromResult<string?>(playerModel.DefaultWalletCurrencyCode));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(gameProviderCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                ).Returns(ValueTask.FromResult(true));

            var actual = await sut.GetBalanceAsync(playerCode, gameProviderCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
