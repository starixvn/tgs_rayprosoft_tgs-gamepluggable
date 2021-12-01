using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameRepository
    {
        ValueTask<GameModel> GetAsync(int gameId);

        ValueTask<GameStatus> GetStatusAsync(int gameId);
    }
}
