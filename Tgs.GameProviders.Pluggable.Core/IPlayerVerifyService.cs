using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerVerifyService
    {
        ValueTask<bool> IsPlayerEnableAsync(string playerCode);

        ValueTask<bool> IsPlayerGameActiveAsync(Guid brandId, int gameId);

        ValueTask<bool> IsPlayerGameProviderActiveAsync(Guid brandId, string gameProviderCode);
    }
}
