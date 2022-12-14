using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Client;

public interface IGameProvidersOperatorPluggableClient
{
    ValueTask GameProviderFundInAsync(string playerCode, string gameProviderCode, decimal amount, CancellationToken cancellationToken = default);

    ValueTask GameProviderFundOutAsync(string playerCode, string gameProviderCode, decimal amount, CancellationToken cancellationToken = default);

    ValueTask<decimal> GetBalanceAsync(string playerCode, string gameProviderCode, CancellationToken cancellationToken = default);

    ValueTask<Uri> GetGameUrlAsync(string playerCode, Guid loginRecordId, bool isMobile, int gameId, string language, string ipAddress, Uri? backUrl = null, CancellationToken cancellationToken = default);

    ValueTask<SM01PlatformLoginResponse> GetGameSM01PlatformLoginInfoAsync(string playerCode, CancellationToken cancellationToken = default);
}
