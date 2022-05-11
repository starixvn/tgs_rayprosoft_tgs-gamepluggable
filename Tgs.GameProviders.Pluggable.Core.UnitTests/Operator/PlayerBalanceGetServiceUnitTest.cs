using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.GameVerification.Client;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class PlayerBalanceGetServiceUnitTest
    {
        [Test]
        public async Task Player_Balance()
        {
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakeGameProvider = Substitute.For<IGameProviderService>();
            var fakePlayer = Substitute.For<IPlayerService>();
            var fakeGameVerificationGrpc = Substitute.For<IGameVerificationGrpc>();

            var fakeService = new PlayerBalanceGetService(
                  fakeGameProvidersClient,
                  fakeGameProvider,
                  fakeGameVerificationGrpc,
                  fakePlayer);

            var query = new PlayerBalanceQuery("4ucmqplxb029", "KY");

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
                 Tgs.GameProviders.QueryServer.Models.GameProviderStatus.Active,
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

            decimal? gameProviderBalance = 1000m;

            _ = fakePlayer.GetAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(player));

            _ = fakeGameProvider.GetAsync(
                query.GameProviderCode).Returns(ValueTask.FromResult(gameProvider));

            _ = fakeGameVerificationGrpc.VerifyPlayerBalance(
                player.PlayerCode, gameProvider.GameProviderId).Returns(true);

            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(player.PlayerCode)).Returns(ValueTask.FromResult(gameProviderBalance));

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
