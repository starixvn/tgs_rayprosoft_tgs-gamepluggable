using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameRepository : IGame
    {
        private readonly IGameProvidersQueryServerClient _client;

        public GameRepository(IGameProvidersQueryServerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameInfo> GetAsync(
            int gameId,
            CancellationToken cancellationToken = default)
        {
            var data = await _client
                    .GameGetAsync(
                        gameId,
                        cancellationToken)
                    .ConfigureAwait(false)
                ?? throw new ArgumentException($"The game doesn't exist.{gameId}");

            return new GameInfo(
                GameId: data.GameId,
                GameExternalId: data.GameExternalId,
                EndpointPath: data.EndpointPath,
                RuleUrl: data.RuleUrl,
                GameProviderId: data.GameProviderId,
                GameType: data.GameType,
                PlatformType: data.PlatformType,
                GameCode: data.GameCode,
                SubGameProviderCode: data.SubGameProviderCode);
        }
    }
}
