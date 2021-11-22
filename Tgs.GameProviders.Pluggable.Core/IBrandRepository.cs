using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IBrandRepository
    {
        ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId);

        ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId);

        ValueTask<BrandStatus> GetStatusAsync(Guid brandId);
    }
}
