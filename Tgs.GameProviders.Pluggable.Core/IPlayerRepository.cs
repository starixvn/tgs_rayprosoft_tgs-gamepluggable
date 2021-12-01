using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerRepository
    {
        ValueTask<PlayerModel> GetAsync(string playerCode);
    }
}
