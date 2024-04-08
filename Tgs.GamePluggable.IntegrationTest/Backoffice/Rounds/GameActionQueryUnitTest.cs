using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Backoffice.Rounds;

public class GameActionQueryUnitTest
{
	[Fact]
	public async Task Round_GameAction_QueryAsync()
	{
		var roundId = Guid.NewGuid();

		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			  {
				  var fakeGameActionQueryService = Substitute.For<IQueryService<GameActionQuery, GameActionResult>>();
				  var query = new GameActionQuery(roundId);
				  _ = fakeGameActionQueryService
					  .QueryAsync(
					  query: Arg.Is(query),
					  cancellationToken: Arg.Any<CancellationToken>())
					  .Returns(Array.Empty<GameActionResult>().ToAsyncEnumerable());

				  _ = services.AddSingleton(fakeGameActionQueryService);
			  }));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersBackofficePluggableClient(httpClient);

		var actual = sut.GameActionQueryAsync(roundId);
		Assert.NotNull(actual);
	}
}
