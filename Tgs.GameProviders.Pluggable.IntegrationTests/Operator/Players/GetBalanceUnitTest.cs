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
            var gameProviderCode = "vr";
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

            var fakeBrandRepository = Substitute.For<IBrandRepository>();
            var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
            var fakeGameRepository = Substitute.For<IGameRepository>();
            var fakeLicenseeRepository = Substitute.For<ILicenseeRepository>();
            var faceLogger = NullLogger<PlayerVerifyService>.Instance;
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();
            var fakeResellerRepository = Substitute.For<IResellerRepository>();

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
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersOperatorPluggableClient(httpClient);

            var service = new PlayerVerifyService(
                     fakeBrandRepository,
                     fakeGameProviderRepository,
                     fakeGameRepository,
                     fakeLicenseeRepository,
                     fakePlayerRepository,
                     faceLogger,
                     fakeResellerRepository
                     );

            _ = fakePlayerRepository
                .GetAsync(playerCode)
                .Returns(ValueTask.FromResult(playerModel));

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

            _ = fakeGameProviderRepository
                .GetAsync(gameProviderCode)
                .Returns(ValueTask.FromResult(gameProviderModel));

            var actual = await sut.GetBalanceAsync(playerCode, gameProviderCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameProviderBalance, actual);
        }
    }
}
