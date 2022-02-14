using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerService
    {
        ValueTask<PlayerInfo> GetAsync(string playerCode);
    }
}
