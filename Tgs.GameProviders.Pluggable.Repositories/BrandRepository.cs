using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class BrandRepository : IBrandRepository
    {
        public ValueTask<GameProviderStatus> GetGameProviderStatusAsync(Guid brandId, Guid gameProviderId) => throw new NotImplementedException();

        public ValueTask<GameStatus> GetGameStatusAsync(Guid brandId, int gameId) => throw new NotImplementedException();

        public ValueTask<BrandStatus> GetStatusAsync(Guid brandId) => throw new NotImplementedException();
    }
}
