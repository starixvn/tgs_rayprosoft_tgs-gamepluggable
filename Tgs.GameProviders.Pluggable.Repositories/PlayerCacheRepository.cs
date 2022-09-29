using Microsoft.Extensions.Caching.Memory;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class PlayerCacheRepository : IPlayer
    {
        private readonly TimeSpan _absoluteExpiration;
        private readonly IMemoryCache _memoryCache;
        private readonly IPlayer _player;

        public PlayerCacheRepository(
            IPlayer player,
            IMemoryCache memoryCache,
            TimeSpan absoluteExpiration
            )
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _absoluteExpiration = absoluteExpiration;
        }

        public async ValueTask<PlayerInfo> GetAsync(
            string playerCode,
            CancellationToken cancellationToken = default)
        {
            var key = $"player:{playerCode}";
            if (!_memoryCache.TryGetValue(key, out PlayerInfo player))
            {
                player = await _player
                        .GetAsync(
                            playerCode,
                            cancellationToken)
                        .ConfigureAwait(false);
                _ = _memoryCache.Set(key, player, _absoluteExpiration);
            }
            return player;
        }

        public ValueTask<decimal?> GetBalanceAsync(string gameProviderCode, string playerCode, string currencyCode, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    }
}
