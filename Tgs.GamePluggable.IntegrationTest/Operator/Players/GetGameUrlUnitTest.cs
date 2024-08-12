using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Operator.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Operator.Players;

public class GetGameUrlUnitTest
{
	[Fact]
	public async Task GetGameUrlAsync()
	{
		var playerCode = "Test";
		var gameId = 1;
		var loginRecordId = Guid.NewGuid();
		var language = "zh-TW";
		var isMobile = false;
		var ipAddress = "127.0.01";
		var backUrl = new Uri("https://dev-t1.rpgp.cc");

		var expected = new Uri("https://localhost/");
		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			  {
				  var fakeGameUrlGetService = Substitute.For<IGetService<GameUrlQuery, Uri>>();

				  var query = new GameUrlQuery(
					  PlayerCode: playerCode,
					  LoginRecordId: loginRecordId,
					  PlatformType: PlatformType.Desktop,
					  GameId: gameId,
					  Language: language,
					  IpAddress: ipAddress,
					  BackUrl: backUrl);

				  _ = fakeGameUrlGetService.GetAsync(
					  query: Arg.Is(query),
					  cancellationToken: Arg.Any<CancellationToken>())
				  .Returns(expected);

				  _ = services.AddSingleton(fakeGameUrlGetService);
			  }));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersOperatorPluggableClient(httpClient);

		var actual = await sut.GetGameUrlAsync(
			playerCode: playerCode,
			loginRecordId: loginRecordId,
			isMobile: isMobile,
			gameId: gameId,
			language: language,
			ipAddress: ipAddress,
			backUrl: backUrl);

		Assert.NotNull(actual);
		Assert.Equal(expected, actual);
	}
}
