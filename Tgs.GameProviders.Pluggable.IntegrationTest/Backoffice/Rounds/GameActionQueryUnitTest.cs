using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.IntegrationTest.Backoffice.Rounds;

public class GameActionQueryUnitTest
{
    [Fact]
    public async Task Round_GameAction_QueryAsync()
    {
        var roundId = Guid.NewGuid();

        await using var web = new TestPluggableApplication(builder =>
        {
            _ = builder.ConfigureServices(services =>
              {
                  var fakeGameActionQueryService = Substitute.For<IQueryService<Guid, GameActionInfo>>();

                  _ = fakeGameActionQueryService
                      .QueryAsync(roundId)
                      .Returns(Array.Empty<GameActionInfo>().ToAsyncEnumerable());

                  _ = services.AddSingleton(fakeGameActionQueryService);
              });
        });

        var httpClient = web.CreateClient();

        var sut = new GameProvidersBackofficePluggableClient(httpClient);

        var actual = sut.GameActionQueryAsync(roundId);
        Assert.NotNull(actual);
    }
}
