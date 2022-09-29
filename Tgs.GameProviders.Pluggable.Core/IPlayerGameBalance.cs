using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerGameBalance
    {
        public ValueTask<decimal> GetBalanceAsync(
            string gameProviderCode,
            string playerCode,
            string currencyCode,
            CancellationToken cancellationToken = default);
    }
}
