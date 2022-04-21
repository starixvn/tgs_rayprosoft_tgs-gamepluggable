using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Client;
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

            var gameProviderModel = await _gameProviderService.GetAsync(roundModel.GameProviderId).ConfigureAwait(false);

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
