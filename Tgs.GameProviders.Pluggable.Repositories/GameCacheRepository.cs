using Microsoft.Extensions.Caching.Memory;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories;

internal class GameCacheRepository : IGame
{
    private readonly TimeSpan _absoluteExpiration;
    private readonly IGame _game;
    private readonly IMemoryCache _memoryCache;

    public GameCacheRepository(
        IGame game,
        IMemoryCache memoryCache,
        TimeSpan absoluteExpiration
        )
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _absoluteExpiration = absoluteExpiration;
    }

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
