using Microsoft.Extensions.Caching.Memory;
using Tgs.GamePluggable.Core;
using Tgs.GamePluggable.Core.Models;

namespace Tgs.GamePluggable.Repositories;

internal class PlayerCacheRepository(
	IPlayerRepository repository,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IPlayerRepository
{
	public async ValueTask<PlayerInfo> GetAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(PlayerCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "player", ":", "get", ":", playerCode);

		if (!memoryCache.TryGetValue(key, out PlayerInfo? playerInfo))
		{
			playerInfo = await repository.GetAsync(
				playerCode: playerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			return memoryCache.Set(key, playerInfo, absoluteExpiration);
		}

		return playerInfo!;
	}

	public ValueTask<bool> GetStatusAsync(string playerCode, CancellationToken cancellationToken = default)
	=> repository.GetStatusAsync(playerCode: playerCode, cancellationToken: cancellationToken);
}
