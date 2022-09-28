using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class PlayerBalanceGetServiceUnitTest
    {
        [Test]
        public async Task Player_Balance()
        {
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakePlayer = Substitute.For<IPlayerService>();

            var fakeService = new PlayerBalanceGetService(
                  fakeGameProvider,
                  fakePlayer);

            var query = new PlayerBalanceQuery("4ucmqplxb029", "KY");

            var player = new PlayerInfo(
                Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
                "GPI",
                "4ucmqplxb029",
                Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
                "CNY");

            var gameProvider = new GameProviderInfo(
                 Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
                 GameProviderStatus.Active,
                 string.Empty,
                 true,
                 true,
                 false,
                 0,
                 0,
                 (int)10,
                 true,
                 GameProviderType.KY,
                 10.ToString(),
                 string.Empty
                );

            decimal? gameProviderBalance = 1000m;

            _ = fakePlayer.GetAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameProvider.GetAsync(
                query.GameProviderCode).Returns(ValueTask.FromResult(gameProvider));

            _ = fakePlayer.GetBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(query.PlayerCode),
                Arg.Is(player.DefaultWalletCurrencyCode)).Returns(gameProviderBalance);

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
