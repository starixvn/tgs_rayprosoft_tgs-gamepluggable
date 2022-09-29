using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IBrand
    {
        ValueTask<Guid> GetGameProviderConfigurationIdAsync(
            Guid brandId,
            Guid gameProviderId,
            CancellationToken cancellationToken = default);
    }
}
