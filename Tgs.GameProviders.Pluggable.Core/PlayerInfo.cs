using System;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record PlayerInfo(
        Guid ResellerId,
        string ResellerCode,
        Guid LicenseeId,
        string LicenseeCode,
        Guid BrandId,
        string BrandCode,
        string PlayerCode,
        string Username,
        Guid DefaultWalletId,
        string DefaultWalletCurrencyCode,
        PlayerStatus Status
     );
}
