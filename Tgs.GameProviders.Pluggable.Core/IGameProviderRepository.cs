using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderModel> GetAsync(Guid gameProviderId);

        ValueTask<GameProviderModel> GetAsync(string gameProviderCode);

        ValueTask<GameProviderStatus> GetStatusAsync(Guid gameProviderId);
    }
}
