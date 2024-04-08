using Microsoft.Extensions.Caching.Memory;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class GameProviderCacheRepository(
	IGameProviderRepository repository,
	IMemoryCache memoryCache) : IGameProviderRepository
{
	public async ValueTask<Core.Models.GameProviderInfo> GetAsync(Guid id, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(GameProviderCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "gameprovider", ":", "get", ":", id);
		if (!memoryCache.TryGetValue(key, out Core.Models.GameProviderInfo? gameProviderInfo))
		{
			gameProviderInfo = await repository.GetAsync(
				id: id,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameProviderInfo is not null)
			{
				return memoryCache.Set(key, gameProviderInfo);
			}
		}

		return gameProviderInfo ?? throw new ArgumentNullException(paramName: id.ToString(), message: $"This game provider does not exist.{id}");
	}

	public ValueTask<Core.Models.GameProviderConfigInfo> GetConfigurationAsync(Guid gameProviderConfigId, CancellationToken cancellationToken = default)
		=> repository.GetConfigurationAsync(gameProviderConfigId: gameProviderConfigId, cancellationToken: cancellationToken);

	public ValueTask<bool> GetCurrencyExistAsync(Guid id, string currencyCode, CancellationToken cancellationToken = default)
		=> repository.GetCurrencyExistAsync(id, currencyCode, cancellationToken);

	public async ValueTask<Guid> GetGameProviderIdAsync(string code, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(GameProviderCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "gameprovider", ":", "get", ":", "id", ":", code);
		if (!memoryCache.TryGetValue(key, out Guid? gameProviderId))
		{
			gameProviderId = await repository.GetGameProviderIdAsync(
				code: code,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			if (gameProviderId.HasValue)
			{
				return memoryCache.Set(key, gameProviderId.Value);
			}
		}

		return gameProviderId ?? throw new ArgumentNullException(paramName: code, message: $"This game provider does not exist.({code})");
	}

	public IAsyncEnumerable<string> QueryAsync(CancellationToken cancellationToken = default)
		=> repository.QueryAsync(cancellationToken);
}
