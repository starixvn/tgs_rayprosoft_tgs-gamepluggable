using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameService
    {
        ValueTask<GameInfo> GetAsync(int gameId);
    }
}
