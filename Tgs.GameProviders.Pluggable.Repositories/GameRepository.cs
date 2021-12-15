using System;
using System.Linq;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;
using Tgs.GameProviders.Pluggable.Repositories.Entities;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameRepository : IGameRepository
    {
        private readonly TgsContext _context;

        public GameRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ValueTask<GameModel> GetAsync(int gameId)
            => _context.Games
            .Where(x => x.Id == gameId)
            .Select(data => new GameModel(data.Id,
                 (GameStatus)data.Status,
                 data.ExternalId,
                 data.GameProviderId,
                 (GameType)data.Type,
                 (GamePlatformType)data.PlatformType,
                 data.SubGame,
                 data.Code,
                 data.Name,
                 data.SubGameProviderCode))
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<GameStatus> GetStatusAsync(int gameId) => _context.Games
            .Where(x => x.Id == gameId)
            .Select(data => (GameStatus)data.Status)
            .ToAsyncEnumerable()
            .FirstAsync();
    }
}
