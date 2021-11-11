using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameRepository
    {
        ValueTask<GameModel> GetAsync(int gameId);

        ValueTask<GameStatus> GetStatusAsync(int gameId);
    }
}
