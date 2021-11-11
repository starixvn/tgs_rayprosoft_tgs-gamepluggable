using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IBrandRepository
    {
        ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId);

        ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId);

        ValueTask<BrandStatus> GetStatusAsync(Guid brandId);
    }
}
