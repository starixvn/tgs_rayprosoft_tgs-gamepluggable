using System.Threading;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core.Models;

namespace Tgs.GameProviders.Pluggable.Core;

public interface IPlayerGameBalance
{
    public ValueTask<PlatformBalanceInfo> GetBalanceAsync(
        string gameProviderCode,
        string playerCode,
        string currencyCode,
        CancellationToken cancellationToken = default);
}
