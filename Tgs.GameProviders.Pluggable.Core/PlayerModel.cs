using System;
using TGS.GameProviders.Domains;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record PlayerModel(
        Guid ResellerId,
        string ResellerCode,
        Guid LicenseeId,
        string LicenseeCode,
        Guid BrandId,
        string BrandCode,
        string PlayerCode,
        string Username,
        string? RealName,
        Guid DefaultWalletId,
        string DefaultWalletCurrencyCode,
        PlayerStatus Status
     );
}
