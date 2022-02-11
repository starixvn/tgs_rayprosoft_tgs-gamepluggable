using System;
using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IBrandService
    {
        ValueTask<BrandGameProviderInfo> GetBrandGameProviderConfigurationAsync(Guid brandId, Guid gameProviderId);

        ValueTask<string?> GetCurrencyCodeAsync(Guid brandId, string currencyCode);

        ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId);

        ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId);

        ValueTask<BrandStatus> GetStatusAsync(Guid brandId);
    }
}
