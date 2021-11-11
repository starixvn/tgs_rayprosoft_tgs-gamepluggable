using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerRepository
    {
        // ValueTask<(Guid LicenseeId, string LicenseeCode, Guid BrandId, string BrandCode, string PlayerCode)> GetAsync(string playerCode);

        ValueTask<PlayerModel> GetAsync(string playerCode);

        ValueTask<int> GetStatusAsync(string playerCode);
    }
}
