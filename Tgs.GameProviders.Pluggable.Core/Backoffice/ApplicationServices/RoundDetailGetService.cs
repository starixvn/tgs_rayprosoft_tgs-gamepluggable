using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices;

internal class RoundDetailGetService(
    IGameRoundsClient gameRoundsClient,
    IGameProvidersClient gameProvidersClient,
    IGameProvider gameProvider) : IGetService<RoundDetailQuery, JsonDocument>
{
    public async ValueTask<JsonDocument> GetAsync(RoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var data = await gameRoundsClient
            .GetGameRoundAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new Exception($"Game Round does not exist.({query})");

        if (data.Status is GameRoundStatus.Open or GameRoundStatus.New
            ||
            data.ClientStatus is GameRoundClientStatus.Open)
        {
            return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataText()
            {
                Value = "The game has not been settled, please wait!(遊戲尚未結算，請稍後!)"
            }));
        }

        var gameProviderModel = await gameProvider
            .GetAsync(
            gameProviderId: data.GameProviderId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var roundDetailResult = await gameProvidersClient
                .RoundDetailAsync(
                gameProviderCode: gameProviderModel!.GameProviderCode,
                roundId: data.RoundId,
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
