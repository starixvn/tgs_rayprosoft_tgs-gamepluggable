using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderModel> GetAsync(Guid gameProviderId);

        ValueTask<GameProviderModel> GetAsync(string gameProviderCode);

        IAsyncEnumerable<GameProviderCurrencyModel> GetCurrenciesAsync(Guid gameProviderId);

        IAsyncEnumerable<GameProviderCurrencyModel> GetCurrenciesAsync(string gameProviderCode);

        ValueTask<GameProviderCurrencyModel?> GetCurrencyAsync(Guid gameProviderId, string currencyCode);

        ValueTask<GameProviderCurrencyModel?> GetCurrencyAsync(string gameProviderCode, string currencyCode);

        ValueTask<GameProviderStatus> GetStatusAsync(Guid gameProviderId);

        ValueTask<SubGameProviderModel> GetSubGameProviderAsync(string subGameProviderCode);
    }
}
