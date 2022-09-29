using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class GameActionQueryService : IQueryService<Guid, GameActionInfo>
{
    private readonly IGameProvidersQueryServerClient _client;

    public GameActionQueryService(IGameProvidersQueryServerClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async IAsyncEnumerable<GameActionInfo> QueryAsync(
        Guid query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var gameActionModels = await _client
                  .GameActionQueryAsync(
                      roundId: query,
                      cancellationToken: cancellationToken)
                  .ConfigureAwait(false);

        if (gameActionModels is null || !gameActionModels.Any())
        {
            yield break;
        }

        foreach (var gameActionModel in gameActionModels)
        {
            yield return new GameActionInfo(
                gameActionModel.Id,
                gameActionModel.RoundId,
                gameActionModel.PlayerCode,
                gameActionModel.ExternalTransactionId,
                gameActionModel.Type,
                gameActionModel.Amount,
                gameActionModel.Turnover,
                gameActionModel.WinLoss,
                gameActionModel.UnsettledBets,
                gameActionModel.Balance,
                gameActionModel.BetType,
                gameActionModel.ExternalBetId,
                gameActionModel.ExternalGameActionGroupId,
                gameActionModel.ReferencedGameActionId,
                gameActionModel.GameActionBatchId,
                gameActionModel.RecordedOn,
                gameActionModel.SentOn,
                gameActionModel.IsClosingRound,
                gameActionModel.ClosingRoundExplicitRequest,
                gameActionModel.PlayerIp,
                gameActionModel.ExtraData,
                gameActionModel.JackpotContribution,
                gameActionModel.StoredOn);
        }
    }
}
