using System;
using System.Threading.Tasks;

namespace Tgs.GameProviders.Pluggable.Core
{
    public interface IPlayerVerifyHanlde
    {
        ValueTask<bool> IsPlayerEnableAsync(PlayerInfo player);

        ValueTask<bool> IsPlayerGameActiveAsync(Guid brandId, GameInfo game);

        ValueTask<bool> IsPlayerGameProviderActiveAsync(Guid brandId, GameProviderInfo gameProvider);

        ValueTask<bool> IsPlayerGameProviderCurrencyAsync(Guid brandId, Guid gameProviderId, string currencyCode);
    }
}
