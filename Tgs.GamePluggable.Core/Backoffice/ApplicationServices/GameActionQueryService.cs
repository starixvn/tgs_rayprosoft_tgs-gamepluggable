using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.ServiceUtility;

namespace Tgs.GamePluggable.Core.Backoffice.ApplicationServices;

internal class GameActionQueryService(IGameCommandClient client) : IQueryService<GameActionQuery, GameActionResult>
{
	public async IAsyncEnumerable<GameActionResult> QueryAsync(GameActionQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var gameActions = await client
			.GameActionQueryAsync(
			roundId: query.RoundId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameActionInfo>();

		if (!gameActions.Any())
		{
			yield break;
		}

		foreach (var gameAction in gameActions)
		{
			yield return new GameActionResult(
				gameAction.Id,
				gameAction.RoundId,
				gameAction.PlayerCode,
				gameAction.ExternalTransactionId,
				gameAction.Type,
				gameAction.Amount,
				gameAction.Turnover,
				gameAction.WinLoss,
				gameAction.UnsettledBets,
				gameAction.Balance,
				gameAction.BetType,
				gameAction.ExternalBetId,
				gameAction.ExternalGameActionGroupId,
				gameAction.ReferencedGameActionId,
				gameAction.GameActionBatchId,
				gameAction.RecordedOn,
				gameAction.SentOn,
				gameAction.IsClosingRound,
				gameAction.ClosingRoundExplicitRequest,
				gameAction.PlayerIp,
				gameAction.ExtraData,
				gameAction.JackpotContribution,
				gameAction.StoredOn);
		}
	}
}
