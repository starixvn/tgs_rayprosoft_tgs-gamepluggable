using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameProviderCacheRepository(
    IGameProvider gameProvider,
    IMemoryCache memoryCache) : IGameProvider
{
    public async ValueTask<GameProviderInfo> GetAsync(
        Guid gameProviderId,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:gameprovidercache:get:{gameProviderId}";
        if (!memoryCache.TryGetValue(key, out GameProviderInfo? gameProviderInfo))
        {
            gameProviderInfo = await gameProvider.GetAsync(
                gameProviderId: gameProviderId,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (gameProviderInfo is not null)
            {
                return memoryCache.Set(key, gameProviderInfo);
            }
        }
        return gameProviderInfo ?? throw new ArgumentException($"game provider doesn't exist.{gameProviderId}");
    }

    public ValueTask<GameProviderConfigInfo> GetConfigurationAsync(
        Guid gameProviderConfigId,
        CancellationToken cancellationToken = default)
        => gameProvider.GetConfigurationAsync(
            gameProviderConfigId: gameProviderConfigId,
            cancellationToken: cancellationToken);

    public async ValueTask<Guid> GetGameProviderIdAsync(
        string gameProviderCode,
        CancellationToken cancellationToken = default)
    {
        var key = $"game:pluggable:gameprovidercache:get:gameproviderid:{gameProviderCode}";
        if (!memoryCache.TryGetValue(key, out Guid? gameProviderId))
        {
            gameProviderId = await gameProvider.GetGameProviderIdAsync(
                gameProviderCode: gameProviderCode,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (gameProviderId.HasValue)
            {
                return memoryCache.Set(key, gameProviderId.Value);
            }
        }
        return gameProviderId ?? throw new Exception($"game provider does not exist.({gameProviderCode})");
    }
}
