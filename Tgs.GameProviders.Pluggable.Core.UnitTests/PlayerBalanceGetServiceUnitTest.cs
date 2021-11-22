using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameProviders.Pluggable.Core.ApplicationServices;
using TGS.GameProviders.Client;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core.UnitTests
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

            var query = new PlayerBalanceQuery()
            {
                GameProviderCode = "ky",
                PlayerCode = "4ucmqplxb029"
            };

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

            var expected = 1000m;

            _ = fakePlayerVerifyService.IsPlayerEnableAsync(query.PlayerCode).Returns(ValueTask.FromResult(true));
            _ = fakePlayerRepository.GetAsync(query.PlayerCode).Returns(ValueTask.FromResult(playerModel));
            _ = fakeGameProvidersClient.PlayerBalanceAsync(
                query.GameProviderCode,
                playerModel.PlayerCode)
                .Returns(ValueTask.FromResult<decimal>(expected));

            var actual = await fakeService.GetAsync(query);

            Assert.AreEqual(expected, actual);
        }
    }
}
