using Microsoft.Extensions.Caching.Memory;
using Tgs.GamePluggable.Core;

namespace Tgs.GamePluggable.Repositories;

internal class GameCacheRepository(
	IGameRepository repository,
	IMemoryCache memoryCache,
	TimeSpan absoluteExpiration) : IGameRepository
{
	private readonly TimeSpan _absoluteExpiration = absoluteExpiration;
	private readonly IGameRepository _game = repository ?? throw new ArgumentNullException(nameof(repository));
	private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

	public async ValueTask<Core.Models.GameInfo> GetAsync(int id, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(GameCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", "game", ":", "get", ":", id);
		if (!_memoryCache.TryGetValue(key, out Core.Models.GameInfo? game))
		{
			game = await _game.GetAsync(
				id: id,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return _memoryCache.Set(key, game, _absoluteExpiration);
		}

		return game! ?? throw new ArgumentNullException(paramName: id.ToString(), message: $"This game does not exist.{id}");
	}

	public IAsyncEnumerable<int> QueryAsync(Guid gameProviderId, CancellationToken cancellationToken = default)
		=> repository.QueryAsync(gameProviderId, cancellationToken);
}
