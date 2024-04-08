using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class GameCacheRepository(
	IGame game,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IGame
{
	private readonly TimeSpan _absoluteExpiration = absoluteExpiration;
	private readonly IGame _game = game ?? throw new ArgumentNullException(nameof(game));
	private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

	public async ValueTask<GameInfo> GetAsync(int gameId, CancellationToken cancellationToken = default)
	{
		var key = $"game:pluggable:gamecache:get:{gameId}";
		if (!_memoryCache.TryGetValue(key, out GameInfo? game))
		{
			game = await _game
					.GetAsync(
						gameId,
						cancellationToken)
					.ConfigureAwait(false);
			_ = _memoryCache.Set(key, game, _absoluteExpiration);
		}

		return game!;
	}
}
