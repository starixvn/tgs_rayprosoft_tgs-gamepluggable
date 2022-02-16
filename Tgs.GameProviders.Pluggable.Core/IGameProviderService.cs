using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameProviderService
    {
        ValueTask<GameProviderInfo> GetAsync(Guid gameProviderId);

        ValueTask<GameProviderInfo> GetAsync(string gameProviderCode);

        ValueTask<string> GetCacheGameProviderCodeAsync(Guid gameProviderId);

        ValueTask<Guid> GetCacheGameProviderIdAsync(string gameProviderCode);

        ValueTask<GameProviderConfigInfo> GetConfigurationAsync(Guid gameProviderConfigId);

        ValueTask<string?> GetCurrencyCodeAsync(Guid gameProviderId, string currencyCode);

        ValueTask<string?> GetCurrencyCodeAsync(string gameProviderCode, string currencyCode);
    }
}
