using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.IntegrationTests.Backoffice.Rounds
{
    [TestClass]
    public class GameActionQueryUnitTest
    {
        [TestMethod]
        public async Task Round_GameAction_QueryAsync()
        {
            var roundId = Guid.NewGuid();

            await using var web = new TestPluggableApplication(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var fakeRoundRepository = Substitute.For<IRoundRepository>();

                    fakeRoundRepository
                        .GameActionQueryAsync(roundId)
                        .Returns(Array.Empty<GameActionInfo>().ToAsyncEnumerable());

                    fakeRoundRepository
                        .GameActionQueryAsync(Arg.Is(roundId))
                        .Returns(Array.Empty<GameActionInfo>().ToAsyncEnumerable());

                    services.AddSingleton(fakeRoundRepository);
                    services.AddFakeNatsMessageQueue();
                });
            });

            var httpClient = web.CreateClient();

            var sut = new GameProvidersBackofficePluggableClient(httpClient);

            var actual = sut.GameActionQueryAsync(roundId);
            Assert.IsNotNull(actual);
        }
    }
}
