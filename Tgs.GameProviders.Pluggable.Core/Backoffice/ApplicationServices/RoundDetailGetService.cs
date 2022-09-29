using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameRounds.Client;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class RoundDetailGetService : IGetService<RoundDetailQuery, JsonDocument>
    {
        private readonly IGameProvider _gameProvider;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameRoundsClient _gameRoundsClient;

        public RoundDetailGetService(
            IGameRoundsClient gameRoundsClient,
            IGameProvidersClient gameProvidersClient,
            IGameProvider gameProvider)
        {
            _gameRoundsClient = gameRoundsClient ?? throw new ArgumentNullException(nameof(gameRoundsClient));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        }

        public async ValueTask<JsonDocument> GetAsync(RoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var data = await _gameRoundsClient
                .GetGameRoundAsync(
                    roundId: query.RoundId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"Game Round does not exist.({query})");

            if (!data.Success || data.Data is null)
            {
                throw new Exception($"Game Round does not exist.({query})");
            }

            if (data.Data.Status is GameRoundStatus.Open or GameRoundStatus.New
                ||
                data.Data.ClientStatus is GameRoundClientStatus.Open)
            {
                return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataText()
                {
                    Value = "The game has not been settled, please wait!(遊戲尚未結算，請稍後!)"
                }));
            }

            var gameProviderModel = await _gameProvider
                        .GetAsync(
                            data.Data.GameProviderId,
                            cancellationToken)
                        .ConfigureAwait(false);

            try
            {
                var roundDetailResult = await _gameProvidersClient
                    .RoundDetailAsync(
                        gameProviderModel!.GameProviderCode,
                        data.Data.RoundId,
                        query.Language,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
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
}
