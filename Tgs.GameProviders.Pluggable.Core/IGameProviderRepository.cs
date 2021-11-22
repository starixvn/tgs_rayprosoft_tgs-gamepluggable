using System;
using System.Threading.Tasks;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IGameProviderRepository
    {
        ValueTask<GameProviderModel> GetAsync(Guid gameProviderId);

        ValueTask<GameProviderModel> GetAsync(string gameProviderCode);

        ValueTask<GameProviderStatus> GetStatusAsync(Guid gameProviderId);

        ValueTask<SubGameProviderModel> GetSubGameProvider(string subGameProviderCode);
    }
}
