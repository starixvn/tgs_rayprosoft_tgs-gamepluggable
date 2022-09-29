using System;

namespace Tgs.GameProviders.Pluggable.Core.Models
{
    public record PlayerInfo(
        Guid BrandId,
        string BrandCode,
        string PlayerCode,
        Guid DefaultWalletId,
        string DefaultWalletCurrencyCode
     );
}
