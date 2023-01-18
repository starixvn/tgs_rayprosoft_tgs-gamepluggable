using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameProviderCacheRepository : IGameProvider
{
    private readonly IGameProvider _gameProvider;
    private readonly IMemoryCache _memoryCache;

    public GameProviderCacheRepository(
        IGameProvider gameProvider,
        IMemoryCache memoryCache
        )
    {
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:gameprovidercache:get:{gameProviderId}";
        if (!_memoryCache.TryGetValue(key, out GameProviderInfo? gameProvider))
        {
            gameProvider = await _gameProvider
                    .GetAsync(
                        gameProviderId,
                        cancellationToken)
                    .ConfigureAwait(false);
            _ = _memoryCache.Set(key, gameProvider);
        }
        return gameProvider!;
    }

    public ValueTask<GameProviderConfigInfo> GetConfigurationAsync(
        Guid gameProviderConfigId,
        CancellationToken cancellationToken = default)
        => _gameProvider
            .GetConfigurationAsync(
                gameProviderConfigId,
                cancellationToken);

    public async ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:gameprovidercache:get:gameproviderid:{gameProviderCode}";
        if (!_memoryCache.TryGetValue(key, out Guid gameProviderId))
        {
            gameProviderId = await _gameProvider
                    .GetGameProviderIdAsync(
                        gameProviderCode,
                        cancellationToken)
                    .ConfigureAwait(false);
            _ = _memoryCache.Set(key, gameProviderId);
        }
        return gameProviderId;
    }
}
