using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    public record GameProviderCurrencyInfo(
        Guid GameProviderId,
        string CurrencyCode,
        string GameProviderCurrencyCode
        );
}
