using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IBrandRepository
{
	ValueTask<Guid> GetGameProviderConfigurationIdAsync(
		Guid brandId,
		Guid gameProviderId,
		CancellationToken cancellationToken = default);

	ValueTask<bool> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default);

	ValueTask<bool> GetGameStatusAsync(Guid brandId, int gameId, CancellationToken cancellationToken = default);

	ValueTask<bool> GetCurrencyExistAsync(Guid brandId, string currencyCode, CancellationToken cancellationToken = default);
}
