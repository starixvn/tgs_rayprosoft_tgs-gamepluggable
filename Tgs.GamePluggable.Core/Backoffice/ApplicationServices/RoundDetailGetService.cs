using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameBets.Client;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class RoundDetailGetService(
	TimeProvider timeProvider,
	IGameBetsClient client,
	IGameProvidersClient gameProvidersClient,
	IGameProviderRepository gameProvider) : IGetService<RoundDetailQuery, JsonDocument>
{
	public async ValueTask<JsonDocument> GetAsync(RoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var currentTime = timeProvider.GetUtcNow();
		var info = await client.GameBetMajorGetAsync(
			externalCreatedOn: currentTime.AddMonths(-6),
			id: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");

		if (info.BetState is GameBets.Common.Enums.BetState.Waiting or GameBets.Common.Enums.BetState.Running)
		{
			return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataJson()
			{
				Value = info.AdditionalData.ToString()
			}));
		}

		var gameProviderModel = await gameProvider.GetAsync(
			id: info.GameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		try
		{
			var roundDetailResult = await gameProvidersClient.RoundDetailAsync(
				gameProviderCode: gameProviderModel!.GameProviderCode,
				roundId: info.Id,
				language: query.Language,
				externalCreatedOn: info.ExternalCreatedOn,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			return JsonDocument.Parse(JsonSerializer.Serialize(roundDetailResult));
		}
		catch
		{
			return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataJson()
			{
				Value = info.AdditionalData.ToString()
			}));
		}
	}
}
