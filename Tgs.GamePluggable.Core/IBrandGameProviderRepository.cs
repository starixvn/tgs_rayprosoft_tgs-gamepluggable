using System;

using System.Threading;

using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core;

public interface IBrandGameProviderRepository
{
	ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default);
}
