using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameProviderRepository : IGameProvider
{
    private readonly IGameCommandClient _client;

    public GameProviderRepository(
        IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderGetAsync(
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new ArgumentException($"game provider doesn't exist.{gameProviderId}");

    public async ValueTask<GameProviderConfigInfo> GetConfigurationAsync(
        Guid gameProviderConfigId,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderConfigurationGetAsync(
            gameProviderConfigId: gameProviderConfigId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new ArgumentException($"game provider config doesn't exist.{gameProviderConfigId}");

    public async ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default)
        => await _client
        .GameProviderGetGameProviderIdAsync(
            gameProviderCode: gameProviderCode,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"game provider does not exist.({gameProviderCode})");
}
