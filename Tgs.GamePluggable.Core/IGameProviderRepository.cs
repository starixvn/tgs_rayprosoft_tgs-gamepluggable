using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IGameProviderRepository
{
	ValueTask<Models.GameProviderInfo> GetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Models.GameProviderConfigInfo> GetConfigurationAsync(Guid gameProviderConfigId, CancellationToken cancellationToken = default);

	ValueTask<Guid> GetGameProviderIdAsync(string code, CancellationToken cancellationToken = default);

	IAsyncEnumerable<string> QueryAsync(CancellationToken cancellationToken = default);

	ValueTask<bool> GetCurrencyExistAsync(Guid id, string currencyCode, CancellationToken cancellationToken = default);
}
