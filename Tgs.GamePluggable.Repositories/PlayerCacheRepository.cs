using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class PlayerCacheRepository(
	IPlayer player,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IPlayer
{
	public async ValueTask<PlayerInfo> GetAsync(
		string playerCode,
		CancellationToken cancellationToken = default)
	{
		var key = $"game:pluggable:playercache:get:{playerCode}";
		if (!memoryCache.TryGetValue(key, out PlayerInfo? playerInfo))
		{
			playerInfo = await player.GetAsync(
				playerCode: playerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			_ = memoryCache.Set(key, playerInfo, absoluteExpiration);
		}

		return playerInfo!;
	}
}
