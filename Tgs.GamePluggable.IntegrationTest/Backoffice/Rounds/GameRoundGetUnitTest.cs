using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GamePluggable.Core.Backoffice.ApplicationServices;
using Tgs.GameProviders.Pluggable.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.IntegrationTest.Backoffice.Rounds;

public class GameRoundGetUnitTest
{
	[Fact]
	public async Task RoundGameRoundGetAsync()
	{
		var roundId = Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02");

		await using var web = new TestPluggableApplication(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeGameRoundGetService = Substitute.For<IGetService<GameRoundQuery, GameRoundResult>>();

				var query = new GameRoundQuery(
					RoundId: roundId);
				var result = new GameRoundResult(
					RoundId: Guid.Parse("583b7720-cc29-414b-217d-08d99fff6f02"),
					ExternalRoundId: "50-1636345294-40676339-2",
					GameProviderId: Guid.Parse("a0f49ce0-2c1d-4f50-9fc5-58b610a30bf5"),
					PlayerCode: "4uf6j0t01809",
					GameId: 1000015,
					CurrencyCode: "CNY",
					Status: Core.Enums.RoundStatus.Open,
					GameProviderConfigurationId: Guid.Parse("fa0d7706-f910-4e98-bf71-bd7084140304"),
					WalletId: Guid.Parse("b5d49e20-d027-4abb-aec2-08d99a806de9"),
					CreatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
					ClosedOn: null,
					UpdatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
					BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
					ExternalCreatedOn: DateTimeOffset.Parse("2021-11-08 12:25:09.973 +0800"),
					ExternalClosedOn: null,
					BetAmount: 0.060000m,
					ValidBetAmount: 0.060000m,
					SettleAmount: null,
					Commission: null,
					Turnover: null,
					WinLoss: null,
					TieBetAdjustment: null,
					ClientStatus: Core.Enums.RoundClientStatus.Open);
				_ = fakeGameRoundGetService
				.GetAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(ValueTask.FromResult(result));

				_ = services.AddSingleton(fakeGameRoundGetService);
			}));

		var httpClient = web.CreateClient();

		var sut = new GameProvidersBackofficePluggableClient(httpClient);

		var actual = await sut.GameRoundGetAsync(roundId: roundId);

		Assert.NotNull(actual);
		Assert.Equal(roundId, actual!.RoundId);
	}
}
