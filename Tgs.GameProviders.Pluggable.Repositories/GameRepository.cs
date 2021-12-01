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
        private readonly TgsContext _tgsContext;

        public GameRepository(TgsContext tgsContext)
        {
            _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
        }

        public ValueTask<GameModel> GetAsync(int gameId)
            => _tgsContext.Games
            .Where(x => x.Id == gameId)
            .Select(data => new GameModel()
            {
                GameId = data.Id,
                Status = (GameStatus)data.Status,
                GameExternalId = data.ExternalId,
                GameProviderId = data.GameProviderId,
                GameType = (GameType)data.Type,
                PlatformType = (GamePlatformType)data.PlatformType,
                SubGame = data.SubGame,
                GameCode = data.Code,
                Name = data.Name,
                SubGameProviderCode = data.SubGameProviderCode
            })
            .ToAsyncEnumerable()
            .FirstAsync();

        public ValueTask<GameStatus> GetStatusAsync(int gameId) => _tgsContext.Games
            .Where(x => x.Id == gameId)
            .Select(data => (GameStatus)data.Status)
            .ToAsyncEnumerable()
            .FirstAsync();
    }
}
