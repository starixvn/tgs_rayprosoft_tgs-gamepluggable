using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IBrandRepository
    {
        IAsyncEnumerable<string> GetCurrenciesAsync(Guid brandId);

        ValueTask<string?> GetCurrencyAsync(Guid brandId, string currencyCode);

        ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId);

        ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId);

        ValueTask<BrandStatus> GetStatusAsync(Guid brandId);

        ValueTask<BrandGameProviderModel> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);
    }
}