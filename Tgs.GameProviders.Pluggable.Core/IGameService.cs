using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameService
    {
        ValueTask<GameInfo> GetAsync(int gameId);

        ValueTask<GameStatus> GetStatusAsync(int gameId);
    }
}
