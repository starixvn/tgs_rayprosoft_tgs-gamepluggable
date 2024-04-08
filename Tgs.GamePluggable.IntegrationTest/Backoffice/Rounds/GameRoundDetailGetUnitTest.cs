using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Backoffice.Rounds;

public class GameRoundDetailGetUnitTest
{
	[Fact]
	public async Task GameRoundDetailGetAsync()
	{
		var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");
		var language = "zh-CN";
		var gameProviderId = Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5");

		var roundDetailResult = new RoundDetailResult()
		{
			Data = null
		};

		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			  {
				  var fakeRoundDetailGetService = Substitute.For<IGetService<RoundDetailQuery, JsonDocument>>();

				  var query = new RoundDetailQuery
				  {
					  RoundId = roundId,
					  Language = language
				  };

				  _ = fakeRoundDetailGetService
					.GetAsync(
						Arg.Is(query))
					.Returns(ValueTask.FromResult(JsonDocument.Parse(JsonSerializer.Serialize(query))));

				  _ = services.AddSingleton(fakeRoundDetailGetService);
			  }));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersBackofficePluggableClient(httpClient);

		var actual = await sut.GameRoundDetailGetAsync(
			roundId: roundId,
			language: language);

		Assert.NotNull(actual);
	}
}
