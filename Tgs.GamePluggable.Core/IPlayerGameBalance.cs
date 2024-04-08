using System.Threading;
using System.Threading.Tasks;
using Tgs.GamePluggable.Core.Models;

namespace Tgs.GamePluggable.Core;

public interface IPlayerGameBalance
{
	public ValueTask<PlatformBalanceInfo> GetBalanceAsync(
		string gameProviderCode,
		string playerCode,
		string currencyCode,
		CancellationToken cancellationToken = default);
}
