using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameRepository : IGameRepository
    {
        public ValueTask<GameModel> GetAsync(int gameId) => throw new NotImplementedException();

        public ValueTask<GameStatus> GetStatusAsync(int gameId) => throw new NotImplementedException();
    }
}
