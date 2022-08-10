using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameRounds.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Pluggable.Core.Backoffice.ApplicationServices
{
    internal class RoundDetailGetService : IGetService<RoundDetailQuery, JsonDocument>
    {
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IGameProviderService _gameProviderService;
        private readonly IRoundRepository _roundRepository;

        public RoundDetailGetService(
            IRoundRepository roundRepository,
            IGameProvidersClient gameProvidersClient,
            IGameProviderService gameProviderService)
        {
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
            _gameProviderService = gameProviderService ?? throw new ArgumentNullException(nameof(gameProviderService));
        }

        public async ValueTask<JsonDocument> GetAsync(RoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);

            if (roundModel == null)
            {
                throw new Exception($"Game Round does not exist.({query})");
            }

            if (roundModel.Status is GameRoundStatus.Open or GameRoundStatus.New
                ||
                roundModel.ClientStatus is GameRoundClientStatus.Open)
            {
                return JsonDocument.Parse(JsonSerializer.Serialize(new RoundDetailDataText()
                {
                    Value = "The game has not been settled, please wait!(遊戲尚未結算，請稍後!)"
                }));
            }

            var gameProviderModel = await _gameProviderService.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);

            try
            {
                var roundDetailResult = await _gameProvidersClient
                    .RoundDetailAsync(
                        gameProviderModel!.GameProviderCode,
                        roundModel.RoundId,
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
