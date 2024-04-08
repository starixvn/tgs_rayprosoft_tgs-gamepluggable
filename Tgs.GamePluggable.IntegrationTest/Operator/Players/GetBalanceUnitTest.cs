using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Operator.Players;

public class GetBalanceUnitTest
{
	[Fact]
	public async Task GetBalanceAsync()
	{
		var playerCode = "4ucmqplxb029";
		var gameProviderCode = "VR";
		var gameProviderId = Guid.NewGuid();

		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakePlayerBalanceGetService = Substitute.For<IGetService<PlayerBalanceQuery, PlayerBalanceResult>>();

				var query = new PlayerBalanceQuery(
					playerCode,
					gameProviderCode);

				var result = new PlayerBalanceResult(
					Balance: 100m,
					FreeBalance: 10m,
					OutstandingBalance: 90m);

				_ = fakePlayerBalanceGetService
					.GetAsync(
					Arg.Is(query),
					Arg.Any<CancellationToken>())
					.Returns(result);

				_ = services.AddSingleton(fakePlayerBalanceGetService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersOperatorPluggableClient(httpClient);

		var actual = await sut.GetBalanceAsync(
			playerCode: playerCode,
			gameProviderCode: gameProviderCode);

		Assert.Equal(10m, actual);
	}
}
