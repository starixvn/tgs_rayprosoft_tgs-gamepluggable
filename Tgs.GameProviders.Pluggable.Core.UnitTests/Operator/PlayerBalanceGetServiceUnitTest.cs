using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core.Operator.ApplicationServices;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests.Operator
{
    [TestFixture]
    public class PlayerBalanceGetServiceUnitTest
    {
        [Test]
        public async Task Player_Balance()
        {
            var faceLogger = NullLogger<PlayerBalanceGetService>.Instance;
            var fakeGameProvidersClient = Substitute.For<IGameProvidersClient>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakePlayerVerifyService = Substitute.For<IPlayerVerifyService>();

            var fakeService = new PlayerBalanceGetService(
                  fakeGameProvidersClient,
                  faceLogger,
                  fakePlayerRepository,
                  fakePlayerVerifyService);

            var query = new PlayerBalanceQuery("KY", "4ucmqplxb029");

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

            decimal? gameProviderBalance = 1000m;

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(true));

            _ = fakePlayerRepository.GetAsync(
                Arg.Is(query.PlayerCode)).Returns(ValueTask.FromResult(playerModel));

            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                Arg.Is(query.GameProviderCode),
                Arg.Is(playerModel.PlayerCode)).Returns(ValueTask.FromResult(gameProviderBalance));

            _ = fakePlayerVerifyService.IsPlayerGameProviderActiveAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(query.GameProviderCode)).Returns(ValueTask.FromResult(true));

            _ = fakePlayerVerifyService.IsPlayerGameProviderCurrencyAsync(
                Arg.Is(playerModel.BrandId),
                Arg.Is(query.GameProviderCode),
                Arg.Is(playerModel.DefaultWalletCurrencyCode)
                ).Returns(ValueTask.FromResult(true));

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
