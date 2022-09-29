using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameCacheRepository : IGame
    {
        private readonly IGame _game;
        private readonly IMemoryCache _memoryCache;

        public GameCacheRepository(
            IGame game,
            IMemoryCache memoryCache
            )
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async ValueTask<GameInfo> GetAsync(int gameId, CancellationToken cancellationToken = default)
        {
            var key = $"game:{gameId}";
            if (!_memoryCache.TryGetValue(key, out GameInfo game))
            {
                game = await _game
                        .GetAsync(
                            gameId,
                            cancellationToken)
                        .ConfigureAwait(false);
                _ = _memoryCache.Set(key, game);
            }
            return game;
        }
    }
}
