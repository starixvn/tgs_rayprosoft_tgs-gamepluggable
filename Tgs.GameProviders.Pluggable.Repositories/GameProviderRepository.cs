using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;
using Tgs.GameProviders.QueryServer.Client;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameProviderRepository : IGameProvider
    {
        private readonly IGameProvidersQueryServerClient _client;

        public GameProviderRepository(
            IGameProvidersQueryServerClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameProviderInfo> GetAsync(
            Guid gameProviderId,
            CancellationToken cancellationToken = default)
        {
            var data = await _client
                    .GameProviderGetAsync(
                        gameProviderId,
                        cancellationToken)
                    .ConfigureAwait(false)
                ?? throw new ArgumentException($"game provider doesn't exist.{gameProviderId}");

            return new GameProviderInfo(
                   GameProviderId: data.GameProviderId,
                   GameProviderCode: data.GameProviderCode);
        }

        public async ValueTask<GameProviderInfo> GetAsync(
            string gameProviderCode,
            CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _client
                    .GameProviderGetGameProviderIdAsync(
                        gameProviderCode,
                        cancellationToken)
                    .ConfigureAwait(false)
                    ?? throw new Exception($"game provider does not exist.({gameProviderCode})");

            return await GetAsync(
                    gameProviderId,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask<GameProviderConfigInfo> GetConfigurationAsync(
            Guid gameProviderConfigId,
            CancellationToken cancellationToken = default)
        {
            var data = await _client
                .GameProviderConfigurationGetAsync(
                    gameProviderConfigId,
                    cancellationToken)
                .ConfigureAwait(false)
                ?? throw new ArgumentException($"game provider config doesn't exist.{gameProviderConfigId}");

            return new GameProviderConfigInfo(
                data.GameProviderConfigId,
                data.GameProviderId,
                data.DesktopEndpoint,
                data.MobileEndpoint,
                data.ConfigurationData);
        }

        public async ValueTask<Guid> GetGameProviderIdAsync(
            string gameProviderCode,
            CancellationToken cancellationToken = default)
            => await _client
                    .GameProviderGetGameProviderIdAsync(
                        gameProviderCode,
                        cancellationToken)
                    .ConfigureAwait(false)
                    ?? throw new Exception($"game provider does not exist.({gameProviderCode})");
    }
}
