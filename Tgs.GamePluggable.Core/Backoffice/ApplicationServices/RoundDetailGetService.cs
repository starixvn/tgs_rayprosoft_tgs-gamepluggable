using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameRounds.Client;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class RoundDetailGetService(
	IGameRoundsClient gameRoundsClient,
	IGameProvidersClient gameProvidersClient,
	IGameProviderRepository gameProvider) : IGetService<RoundDetailQuery, JsonDocument>
{
	public async ValueTask<JsonDocument> GetAsync(RoundDetailQuery query, CancellationToken cancellationToken = default)
	{
		var round = await gameRoundsClient.RoundGetAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(query.RoundId.ToString(), $"This game bet does not exist.");

		if (round.Status is GameRounds.Common.Enums.RoundStatus.Open or GameRounds.Common.Enums.RoundStatus.New
			||
			round.ClientStatus is GameRounds.Common.Enums.RoundClientStatus.Open)
		{
			return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataText()
			{
				Value = "The game has not been settled, please wait!(遊戲尚未結算，請稍後!)"
			}));
		}

		var gameProviderModel = await gameProvider
			.GetAsync(
			id: round.GameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		try
		{
			var roundDetailResult = await gameProvidersClient
				.RoundDetailAsync(
				gameProviderCode: gameProviderModel!.GameProviderCode,
				roundId: round.RoundId,
				language: query.Language,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			return JsonDocument.Parse(JsonSerializer.Serialize(roundDetailResult));
		}
		catch
		{
			return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataText()
			{
				Value = "Game result not yet, please wait!(遊戲結果尚未，請稍後!)"
			}));
		}
	}
}
