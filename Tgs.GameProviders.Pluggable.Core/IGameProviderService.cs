using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameProviderService
    {
        ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId);

        ValueTask<GameProviderInfo> GetAsync(string gameProviderCode);

        ValueTask<Guid> GetCacheGameProviderIdAsync(string gameProviderCode);

        ValueTask<GameProviderConfigInfo> GetConfigurationAsync(Guid gameProviderConfigId);
    }
}
