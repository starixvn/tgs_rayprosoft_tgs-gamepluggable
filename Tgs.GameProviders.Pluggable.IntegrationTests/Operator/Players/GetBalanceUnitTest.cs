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
            var gameProviderId = Guid.NewGuid();
            decimal? gameProviderBalance = 0.88m;
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
                QueryServer.Models.PlayerStatus.Enabled
                );

            var gameProvider = new GameProviderInfo(
               Guid.Parse("ae5533bd-35d5-5478-19d6-233ed44f7c9d"),
               QueryServer.Models.GameProviderStatus.Active,
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

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();

                    _ = fakeGameProvidersClient
                     .PlayerBalanceAsync(gameProviderCode, playerCode)
                     .Returns(ValueTask.FromResult(gameProviderBalance));

                    _ = services.AddSingleton(fakeGameProvidersClient);

                    var fakePlayer = Substitute.For<IPlayerService>();

                    _ = fakePlayer
                        .GetAsync(playerCode)
                        .Returns(ValueTask.FromResult(player));

                    _ = services.AddSingleton(fakePlayer);

                    var fakeBrand = Substitute.For<IBrandService>();

                    _ = fakeBrand
                        .GetStatusAsync(player.BrandId)
                        .Returns(ValueTask.FromResult(BrandStatus.Active));
                    _ = fakeBrand.GetCurrencyCodeAsync(
                        Arg.Is(player.BrandId),
                        Arg.Is(player.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult<string?>(player.DefaultWalletCurrencyCode));

                    _ = services.AddSingleton(fakeBrand);

                    var fakeLicensee = Substitute.For<ILicenseeService>();

                    _ = fakeLicensee
                        .GetStatusAsync(player.LicenseeId)
                        .Returns(ValueTask.FromResult(LicenseeStatus.Active));

                    _ = services.AddSingleton(fakeLicensee);

                    var fakeReseller = Substitute.For<IResellerService>();

                    _ = fakeReseller
                        .GetStatusAsync(player.ResellerId)
                        .Returns(ValueTask.FromResult(ResellerStatus.Active));

                    _ = services.AddSingleton(fakeReseller);

                    var fakeGameProvider = Substitute.For<IGameProviderService>();

                    _ = fakeGameProvider.GetAsync(
                        Arg.Is(gameProviderCode)).Returns(ValueTask.FromResult(gameProvider));

                    _ = fakeGameProvider.GetCurrencyCodeAsync(
                        Arg.Is(gameProvider.GameProviderId),
                        Arg.Is(player.DefaultWalletCurrencyCode)).Returns(ValueTask.FromResult<string?>(player.DefaultWalletCurrencyCode));

                    _ = services.AddSingleton(fakeGameProvider);

                    var fakePlayerVerify = Substitute.For<IPlayerVerifyHanlde>();
                    _ = fakePlayerVerify.IsPlayerEnableAsync(
                        player).Returns(ValueTask.FromResult(true));

                    _ = fakePlayerVerify.IsPlayerGameProviderActiveAsync(
                        player.BrandId,
                        gameProvider).Returns(ValueTask.FromResult(true));

                    _ = fakePlayerVerify.IsPlayerGameProviderCurrencyAsync(
                        player.BrandId,
                        gameProvider.GameProviderId,
                        player.DefaultWalletCurrencyCode).Returns(ValueTask.FromResult(true));

                    _ = services.AddSingleton(fakePlayerVerify);

                    _ = services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            var actual = await sut.GetBalanceAsync(playerCode, gameProviderCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
