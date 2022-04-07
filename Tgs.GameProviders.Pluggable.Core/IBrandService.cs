using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IBrandService
    {
        ValueTask<BrandGameProviderInfo> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);
    }
}
