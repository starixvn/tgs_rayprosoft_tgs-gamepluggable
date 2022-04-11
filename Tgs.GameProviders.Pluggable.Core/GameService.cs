using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Core
{
    internal class GameService : IGameService
    {
        private readonly IGameProvidersQueryServerClient _gameProvidersQueryServerClient;

        public GameService(IGameProvidersQueryServerClient gameProvidersQueryServerClient)
        {
            _gameProvidersQueryServerClient = gameProvidersQueryServerClient ?? throw new ArgumentNullException(nameof(gameProvidersQueryServerClient));
        }

        public async ValueTask<GameInfo> GetAsync(int gameId)
        {
            var data = await _gameProvidersQueryServerClient.GameGetAsync(gameId).ConfigureAwait(false);

            return data != null
                ? (new(
                    data.GameId,
                    data.Status,
                    data.GameExternalId,
                    data.Description,
                    data.EndpointPath,
                    data.RuleUrl,
                    data.GameProviderId,
                    data.GameType,
                    data.PlatformType,
                    data.SubGame,
                    data.AvailableToBrand,
                    data.HasTurnoverOnTieBet,
                    data.Html5,
                    data.AvailableCurrencies,
                    data.GameCode,
                    data.Name,
                    data.Localization,
                    data.SubGameProviderCode))
                : throw new ArgumentException($"The game doesn't exist.{gameId}");
        }
    }
}
