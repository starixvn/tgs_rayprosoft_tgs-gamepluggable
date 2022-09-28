using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record PlayerInfo(
        Guid BrandId,
        string BrandCode,
        string PlayerCode,
        Guid DefaultWalletId,
        string DefaultWalletCurrencyCode
     );
}
