using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IPlayerGameBalance
{
	public ValueTask<Models.PlatformBalanceInfo> GetBalanceAsync(
		string gameProviderCode,
		string playerCode,
		string currencyCode,
		CancellationToken cancellationToken = default);
}
