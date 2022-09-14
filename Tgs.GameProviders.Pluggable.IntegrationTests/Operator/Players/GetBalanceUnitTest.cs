using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameVerification.Client;

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
               (int)11,
               true,
               GameProviderType.VR,
               11.ToString(),
               string.Empty
              );

            await using var web = new TestPluggableApplication(builder =>
            {
                _ = builder.ConfigureServices(services =>
                {
                    var fakePlayer = Substitute.For<IPlayerService>();

                    _ = fakePlayer
                        .GetAsync(Arg.Is(playerCode))
                        .Returns(ValueTask.FromResult(player));

                    _ = fakePlayer
                        .GetBalanceAsync(
                        Arg.Is(gameProviderCode),
                        Arg.Is(playerCode),
                        Arg.Is(player.DefaultWalletCurrencyCode))
                        .Returns(ValueTask.FromResult(gameProviderBalance));

                    _ = services.AddSingleton(fakePlayer);

                    var fakeGameProvider = Substitute.For<IGameProviderService>();

                    _ = fakeGameProvider.GetAsync(
                        Arg.Is(gameProviderCode)).Returns(ValueTask.FromResult(gameProvider));

                    _ = services.AddSingleton(fakeGameProvider);

                    var fakeGameVerification = Substitute.For<IGameVerificationClient>();

                    _ = fakeGameVerification.VerifyPlayerBalanceAsync(
                        player.PlayerCode, gameProvider.GameProviderId).Returns(true);

                    _ = services.AddSingleton(fakeGameVerification);
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
