using System;
using System.Threading.Tasks;
using Tgs.GameProviders.Pluggable.Core;

namespace Tgs.GameProviders.Pluggable.Repositories
{
    internal class GameProviderRepository : IGameProviderRepository
    {
        public ValueTask<GameProviderModel> GetAsync(Guid gameProviderId) => throw new NotImplementedException();

        public ValueTask<GameProviderModel> GetAsync(string gameProviderCode) => throw new NotImplementedException();

        public ValueTask<GameProviderStatus> GetStatusAsync(Guid gameProviderId) => throw new NotImplementedException();

        public ValueTask<SubGameProviderModel> GetSubGameProvider(string subGameProviderCode) => throw new NotImplementedException();
    }
}
