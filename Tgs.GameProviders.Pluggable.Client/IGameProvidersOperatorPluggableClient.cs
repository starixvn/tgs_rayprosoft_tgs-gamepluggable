using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Client;

public interface IGameProvidersOperatorPluggableClient
{
	ValueTask GameProviderFundInAsync(string playerCode, string gameProviderCode, decimal amount, CancellationToken cancellationToken = default);

	ValueTask GameProviderFundOutAsync(string playerCode, string gameProviderCode, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<decimal> GetBalanceAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);

	[Obsolete("遊戲商停業，請進行移除")]
	ValueTask<SM01PlatformLoginResponse> GetGameSM01PlatformLoginInfoAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<Uri> GetGameUrlAsync(string playerCode, Guid loginRecordId, bool isMobile, int gameId, string language, string ipAddress, Uri? backUrl = null, object? extraInfo = null, CancellationToken cancellationToken = default);
}
