using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerService
    {
        ValueTask<PlayerInfo> GetAsync(
            string playerCode,
            CancellationToken cancellationToken = default);

        ValueTask<decimal?> GetBalanceAsync(
            string gameProviderCode,
            string playerCode,
            string currencyCode,
            CancellationToken cancellationToken = default);
    }
}
