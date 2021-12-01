using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.GameProviders.Client;

namespace Tgs.GameProviders.Pluggable.Core.ApplicationServices
{
    internal class RoundDetailGetService : IGetService<RoundDetailQuery, JsonDocument>
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameProvidersClient _gameProvidersClient;
        private readonly IRoundRepository _roundRepository;

        public RoundDetailGetService(
            IGameProviderRepository gameProviderRepository,
            IRoundRepository roundRepository,
            IGameProvidersClient gameProvidersClient)
        {
            _gameProviderRepository = gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
            _gameProvidersClient = gameProvidersClient ?? throw new ArgumentNullException(nameof(gameProvidersClient));
        }

        public async ValueTask<JsonDocument> GetAsync(RoundDetailQuery query, CancellationToken cancellationToken = default)
        {
            var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);

            if (roundModel == null)
            {
                throw new Exception($"Game Round does not exist.({query})");
            }

            var gameProviderModel = await _gameProviderRepository.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);

            var roundDetailResult = await _gameProvidersClient
                .RoundDetailAsync(
                gameProviderModel!.GameProviderCode,
                roundModel.RoundId,
                query.Language)
                .ConfigureAwait(false);

            return JsonDocument.Parse(JsonSerializer.Serialize(roundDetailResult));
        }
    }
}
